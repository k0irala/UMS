﻿namespace UMS.Models
{
    public class EmailSettingsModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }   
        public string DisplayName { get; set; } = string.Empty;
    }
}
