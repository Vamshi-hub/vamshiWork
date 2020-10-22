using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace astorWorkShared.Models
{
    public class ForgeElement
    {
        public int ObjectId { get; set; }
        public string Name { get; set; }
        public List<ForgeElement> Objects { get; set; }

        public List<int> Flatten()
        {
            var result = new List<int> { ObjectId };

            if (Objects != null)
            {
                foreach(var forgeElement in Objects)
                {
                    result.AddRange(forgeElement.Flatten());
                }
            }

            return result;
        }
    }
}
