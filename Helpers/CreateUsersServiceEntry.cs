using System;
using System.Text;
using AuthorizationServer.GraphQL;
using AuthorizationServer.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Threading.Tasks;

// Deprecated as of 6.29.21
namespace AuthorizationServer.Helpers
{
    public class UserServiceHelpers
    {
        private IGraphQLClient _graphQLClient;
       public UserServiceHelpers(IGraphQLClient graphQLClient)
       {
           _graphQLClient = graphQLClient;
       }    

            public async Task CreateUserOnUserService(AuthUserModel userModel)
            {
                    var createUserRequest = new GraphQLRequest {
                        Query = @"
                            mutation($user: UserInput!){
                                createUser(user: $user){
                                    email
                                }
                            }
                        ",
                        Variables = new {user = new UserInputType {Id = userModel.Id, Guid = userModel.GUID ,DisplayName = "", FirstName = "", LastName = "" , Email = userModel.Email}}

                    };                  
            var graphQLResponse = await _graphQLClient.SendMutationAsync<UserInputType>(createUserRequest);
             await Task.FromResult(graphQLResponse);
          //  Console.WriteLine(graphQLResponse.Result.Data.ToString());
            Console.WriteLine(graphQLResponse.Errors);
            }
            
        }
        
    }
