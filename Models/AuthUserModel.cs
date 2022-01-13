using System;

namespace AuthorizationServer.Models
{
    public class AuthUserModel
    {
        public int Id { get; set; }
        public Guid GUID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}