﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Entities.CrmEntities
{
    public class EntityReferenceWrapper
    {
        public Guid Id { get; set; }
        public string LogicalName { get; set; }
        public string Name { get; set; }
    }
}
