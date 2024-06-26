﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using permaAPI.services;


namespace permaAPI
{

    public class MsGraphClaimsTransformation
    {
        private readonly MsGraphService _msGraphService;

        public MsGraphClaimsTransformation(MsGraphService msGraphService)
        {
            _msGraphService = msGraphService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity claimsIdentity = new();
            var groupClaimType = "group";
            if (!principal.HasClaim(claim => claim.Type == groupClaimType))
            {
                var objectidentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
                var objectIdentifier = principal.Claims.FirstOrDefault(t => t.Type == objectidentifierClaimType);

                if (objectIdentifier != null)
                {
                    var groupIds = await _msGraphService.GetGraphApiUserMemberGroups(objectIdentifier.Value);

                    foreach (var groupId in groupIds.ToList())
                    {
                        claimsIdentity.AddClaim(new Claim(groupClaimType, groupId.DisplayName));
                    }
                }
            }

            principal.AddIdentity(claimsIdentity);
            return principal;
        }
    }

}