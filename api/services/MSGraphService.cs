using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using permaAPI.models.users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace permaAPI.services;

public class MsGraphService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly string _aadIssuerDomain;
    private readonly string _aadB2CIssuerDomain;

    public MsGraphService(IConfiguration configuration)
    {
        string[]? scopes = configuration.GetValue<string>("AzureAdB2C:Scopes")?.Split(' ');
        var tenantId = configuration.GetValue<string>("AzureAdB2C:TenantId");

        // Values from app registration
        var clientId = configuration.GetValue<string>("AzureAdB2C:ClientId");
        var clientSecret = configuration.GetValue<string>("AzureAdB2C:UserSecretsValue");

        _aadIssuerDomain = configuration.GetValue<string>("AzureAdB2C:Domain");
        _aadB2CIssuerDomain = configuration.GetValue<string>("AzureAdB2C:Domain");

        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };

        // https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
        var clientSecretCredential = new ClientSecretCredential(
            tenantId, clientId, clientSecret, options);

        _graphServiceClient = new GraphServiceClient(clientSecretCredential, scopes);
    }

    public async Task<User> GetGraphApiUser(string userId)
    {
        return await _graphServiceClient.Users[userId]

            .GetAsync();
    }

    public async Task<UserCollectionResponse> GetUsers()
    {

        return await _graphServiceClient.Users

            .GetAsync();
    }
    public async Task<UserCollectionResponse> GetGraphApiUsersbyPrincipalName(string emailAddress)
    {
        return await _graphServiceClient.Users

            .GetAsync(requestConfiguration =>
            {

                requestConfiguration.QueryParameters.Filter = $"userPrincipalName eq '{emailAddress}'";
            });
    }
    public async Task<UserCollectionResponse> GetGraphApiUsersbyEmail(string emailAddress)
    {
        return await _graphServiceClient.Users

        .GetAsync(requestConfiguration =>
        {

            requestConfiguration.QueryParameters.Filter = $"mail eq '{emailAddress}'";
        });
    }
    public async Task<AppRoleAssignmentCollectionResponse> GetGraphApiUserAppRoles(string userId)
    {
        return await _graphServiceClient.Users[userId]
            .AppRoleAssignments

            .GetAsync();
    }


    public async Task<(string Upn, string Password, string Id)> CreateAzureB2CSameDomainUserAsync(UserModelB2CTenant userModel)
    {
        //if (!userModel.UserPrincipalName.ToLower().EndsWith(_aadB2CIssuerDomain.ToLower()))
        //{
        //    throw new ArgumentException("incorrect Email domain");
        //}

        var password = GetEncodedRandomString();
        var user = new User
        {
            AccountEnabled = true,
            UserPrincipalName = userModel.UserPrincipalName,
            DisplayName = userModel.DisplayName,
            Surname = userModel.Surname,
            GivenName = userModel.GivenName,
            PreferredLanguage = userModel.PreferredLanguage,
            MailNickname = userModel.DisplayName,
            PasswordProfile = new PasswordProfile
            {
                ForceChangePasswordNextSignIn = true,
                Password = password
            }
        };

        await _graphServiceClient.Users.PostAsync(user);

        //    .AddAsync(user);

        // Needs an SPO license
        //var patchValues = new User()
        //{
        //    Birthday = userModel.BirthDate.ToUniversalTime()
        //};

        //var request = _graphServiceClient.Users[createdUser.Id].Request();
        //await request.UpdateAsync(patchValues);

        return (user.UserPrincipalName, user.PasswordProfile.Password, user.Id);
    }


    public async Task<(string Upn, string Password, string Id)> CreateUserWithCustomDomain(UserModelB2CIdentity userModel)
    {
        // new user create, email does not matter unless you require to send mails
        var password = GetEncodedRandomString();
        var user = new User
        {
            DisplayName = userModel.DisplayName,
            PreferredLanguage = userModel.PreferredLanguage,
            Surname = userModel.Surname,
            GivenName = userModel.GivenName,
            OtherMails = new List<string> { userModel.Email },

            Identities = new List<ObjectIdentity>()
            {
                new ObjectIdentity
                {
                    SignInType = "emailAddress",
                    Issuer = _aadB2CIssuerDomain,
                    IssuerAssignedId = userModel.Email
                },
            },
            PasswordProfile = new PasswordProfile
            {
                Password = password,
                ForceChangePasswordNextSignIn = false
            },
            PasswordPolicies = "DisablePasswordExpiration",
            Mail = userModel.Email
        };

        var createdUser = await _graphServiceClient.Users
             .PostAsync(user);


        return (createdUser.UserPrincipalName, user.PasswordProfile.Password, createdUser.Id);
    }

    public async Task<(string Upn, string Password, string Id)> CreateFederatedUserWithPasswordAsync(UserModelB2CIdentity userModel)
    {
        // new user create, email does not matter unless you require to send mails
        var password = GetEncodedRandomString();
        var user = new User
        {
            DisplayName = userModel.DisplayName,
            PreferredLanguage = userModel.PreferredLanguage,
            Surname = userModel.Surname,
            GivenName = userModel.GivenName,
            OtherMails = new List<string> { userModel.Email },


            Identities = new List<ObjectIdentity>()
            {
                new ObjectIdentity
                {
                    SignInType = "federated",
                    Issuer = _aadB2CIssuerDomain,
                    IssuerAssignedId = userModel.Email
                },
            },
            PasswordProfile = new PasswordProfile
            {
                Password = password,
                ForceChangePasswordNextSignIn = false
            },
            PasswordPolicies = "DisablePasswordExpiration"
        };

        var createdUser = await _graphServiceClient.Users

            .PostAsync(user);

        return (createdUser.UserPrincipalName, user.PasswordProfile.Password, createdUser.Id);
    }

    public async Task<string> CreateFederatedNoPasswordAsync(UserModelB2CIdentity userModel)
    {
        // User must already exist in AAD
        var user = new User
        {
            DisplayName = userModel.DisplayName,
            PreferredLanguage = userModel.PreferredLanguage,
            Surname = userModel.Surname,
            GivenName = userModel.GivenName,
            OtherMails = new List<string> { userModel.Email },
            Identities = new List<ObjectIdentity>()
            {
                new ObjectIdentity
                {
                    SignInType = "federated",
                    Issuer = _aadIssuerDomain,
                    IssuerAssignedId = userModel.Email
                },
            }
        };

        var createdUser = await _graphServiceClient.Users

            .PostAsync(user);

        return createdUser.UserPrincipalName;
    }

    /// <summary>
    /// Graph invitations only works for Azure AD, not Azure B2C
    /// </summary>
    /// <param name="email"></param>
    /// <param name="redirectUrl"></param>
    /// <returns></returns>
    public async Task<Invitation> InviteUser(string email, string redirectUrl)
    {
        var invitation = new Invitation
        {
            InvitedUserEmailAddress = email,
            InviteRedirectUrl = redirectUrl,
            InvitedUserType = "Member" // default is guest
        };

        var invite = await _graphServiceClient.Invitations

            .PostAsync(invitation);

        return invite;
    }

    public bool IsEmailValid(string email)
    {
        if (!MailAddress.TryCreate(email, out var mailAddress))
            return false;

        // And if you want to be more strict:
        var hostParts = mailAddress.Host.Split('.');
        if (hostParts.Length == 1)
            return false; // No dot.
        if (hostParts.Any(p => p == string.Empty))
            return false; // Double dot.
        if (hostParts[^1].Length < 2)
            return false; // TLD only one letter.

        if (mailAddress.User.Contains(' '))
            return false;
        if (mailAddress.User.Split('.').Any(p => p == string.Empty))
            return false; // Double dot or dot at end of user part.

        return true;
    }

    private static string GetEncodedRandomString()
    {
        string[] words = { "app", "fru", "ban", "san", "lop", "tre", "nes", "her", "fan", "gra", "pol", "mun", "che", "sor", "dra", "dwa", "cen" };


        var base64 = words[System.Security.Cryptography.RandomNumberGenerator.GetInt32(words.Length - 1)] + Convert.ToBase64String(GenerateRandomBytes(4)) + words[System.Security.Cryptography.RandomNumberGenerator.GetInt32(words.Length - 1)];
        return HtmlEncoder.Default.Encode(base64);
    }

    private static byte[] GenerateRandomBytes(int length)
    {
        var item = RandomNumberGenerator.Create();
        var byteArray = new byte[length];
        item.GetBytes(byteArray);
        return byteArray;
    }

    public async Task<List<Group>> GetGraphApiUserMemberGroups(string userId)
    {
        // Fetch the member groups
        var memberObjects = await _graphServiceClient.Users[userId].MemberOf
            .GetAsync();

        var groupsList = new List<Group>();

        // Check each member object to see if it's a security-enabled group
        foreach (var directoryObject in memberObjects.Value)
        {
            if (directoryObject is Group group && group.SecurityEnabled == true)
            {
                groupsList.Add(group);
            }
        }

        // If there are more pages of data, you could loop through them using the .NextPageRequest.GetAsync() method
        // Note: Implementing pagination handling is recommended for production code

        return groupsList;
    }

}