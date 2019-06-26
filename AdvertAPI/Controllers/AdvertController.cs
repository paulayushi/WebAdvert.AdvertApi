using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertAPI.Models;
using AdvertAPI.Models.Messages;
using AdvertAPI.Services;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AdvertAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertServiceStore _advertServiceStore;
        private readonly IConfiguration _configuration;

        public AdvertController(IAdvertServiceStore advertServiceStore, IConfiguration configuration)
        {
            _advertServiceStore = advertServiceStore;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(404)]
        [ProducesResponseType(201, Type = typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            string recordId;
            try
            {
                recordId = await _advertServiceStore.Add(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return StatusCode(201, new CreateAdvertResponse { Id = recordId });
        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(AdvertConfirmModel model)
        {
            try
            {
                await _advertServiceStore.Confirm(model);
                await RaiseAdvertConfirmedEvent(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return new OkResult();
        }

        [HttpGet]
        [Route("All")]
        [ProducesResponseType(200)]
        [EnableCors("AllOrigin")]
        public async Task<IActionResult> All()
        {
            try
            {
                return new JsonResult( await _advertServiceStore.GetAll());
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private async Task RaiseAdvertConfirmedEvent(AdvertConfirmModel model)
        {
            var snsTopicUrn = _configuration.GetValue<string>("AdvertApiSnsTopic");
            var dbContext = await _advertServiceStore.GetById(model.Id);

            using (var client = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.USEast1))
            {
                var message = new AdvertConfirmedMessages
                {
                    Id = model.Id,
                    Title = dbContext.Title
                };
                var messageJson = JsonConvert.SerializeObject(message);
                await client.PublishAsync(snsTopicUrn, messageJson);
            }
        }
    }
}