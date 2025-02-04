﻿using System;
using System.Collections.Generic;

namespace CollegeBackend.Objects.Database;

public partial class Train
{
    public Train()
    {
        Actives = new HashSet<Active>();
        Carriages = new HashSet<Carriage>();
    }

    public int TrainId { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Active> Actives { get; set; }
    public virtual ICollection<Carriage> Carriages { get; set; }
}