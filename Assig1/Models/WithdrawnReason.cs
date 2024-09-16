﻿using System;
using System.Collections.Generic;

namespace Assig1.Models;

public partial class WithdrawnReason
{
    public string WithdrawCode { get; set; } = null!;

    public string WithdrawReason { get; set; } = null!;

    public virtual ICollection<Expiation> Expiations { get; set; } = new List<Expiation>();
}