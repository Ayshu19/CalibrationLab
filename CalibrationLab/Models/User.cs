namespace CalibrationLab.Models
{
    public class User
    {
        public string Name { get; set; }
        public string EmployeeId { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public byte[] Signature { get; set; }
    }
}