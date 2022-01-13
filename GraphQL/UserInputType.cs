using System;

namespace AuthorizationServer.GraphQL
{
    public class UserInputType
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

}