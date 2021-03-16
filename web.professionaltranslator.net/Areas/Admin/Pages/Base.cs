﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace web.professionaltranslator.net.Areas.Admin.Pages
{
    [Authorize(Roles = "Administrator")]
    public class Base : net.Pages.Base
    {
    }
}
