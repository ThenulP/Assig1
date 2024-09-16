﻿using System;
using System.Collections.Generic;

namespace Assig1.Models;

public partial class NoticeState
{
    public string StatusCode { get; set; } = null!;

    public string StatusDescription { get; set; } = null!;

    public virtual ICollection<Expiation> Expiations { get; set; } = new List<Expiation>();
}