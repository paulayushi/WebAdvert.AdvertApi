using AdvertAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertAPI.Services
{
    interface IAdvertServiceStore
    {
        Task<string> Add(AdvertModel model);
        Task Confirm(AdvertConfirmModel model);
    }
}
