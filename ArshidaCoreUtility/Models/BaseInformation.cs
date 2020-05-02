﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ArshidaCoreUtility.Models
{
    public class SoftwareInfo:Resource
    {
        public string SoftwareName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhones { get; set; }
        public string CompanyCellPhones { get; set; }
        public string CompanyAddress { get; set; }
        public float SoftwareVersion { get; set; }
    }
}
