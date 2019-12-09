﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Geonorge.MinSide.Services.Authorization
{
    public class BaatAuthzUserRolesResponse
    {
        public static readonly BaatAuthzUserRolesResponse Empty = new BaatAuthzUserRolesResponse();

        [JsonProperty("services")]
        public List<string> Services = new List<string>();
    }
}
