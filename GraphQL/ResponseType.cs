using System;

namespace AuthorizationServer.GraphQL
{
    public class ResponseType
    {
        public UserResponseType UserResponseType { get; set; }
    }
    public class UserResponseType
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Email { get; set; }        
    }
}