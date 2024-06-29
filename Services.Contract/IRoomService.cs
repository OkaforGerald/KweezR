using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedAPI.TransferObjects;

namespace Services.Contract
{
    public interface IRoomService
    {
        Task<List<RoomsDto>> GetRoomsAsync();
    }
}
