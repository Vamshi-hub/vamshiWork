﻿using System;

namespace astorWorkDAO
{
    public class MaterialQCPhotos
    {
        public int ID { get; set; }
        public string URL { get; set; }
        public string Remarks { get; set; }
        public bool IsOpen { get; set; }

        public int? CreatedById { get; set; }
        public UserMaster CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
