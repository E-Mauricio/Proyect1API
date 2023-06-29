﻿using System.ComponentModel.DataAnnotations;

namespace Proyect1API.Models
{
    public class ConnectedDevice
    {
        
        public string PairedDeviceId { get; set; }
        [Key]
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }

    }
}
