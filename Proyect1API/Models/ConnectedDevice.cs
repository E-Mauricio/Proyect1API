using System.ComponentModel.DataAnnotations;

namespace Proyect1API.Models
{
    public class ConnectedDevice
    {
        [Key]
        public string PairedDeviceId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }

    }
}
