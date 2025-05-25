using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.Domain.DS.DSItems
{
    public class DSItemAddReq
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class DSItemAddWithSubItemReq
    {
        public string Name { get; set; }
        public string? SubName { get; set; }
    }

    public class DSItemWithSubDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<DSItemSubDto> DSItemSubDtos { get; set; }
        public DSItemWithSubDto()
        {
            DSItemSubDtos = new List<DSItemSubDto>();
        }
    }

    public class DSItemWithSubDtoV2
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string SubName { get; set; }
        public int SubID { get; set; }
    }

    public class DSItemWithSubDtoV3
    {
        public int ItemID { get; set; }
        public int ItemSubID { get; set; }
        public string Name { get; set; }
    }

    public class DSItemDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
