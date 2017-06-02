using System;
using System.Collections.Generic;
using System.Linq;
using DeusCloud.Exceptions;
using WispCloud;

namespace DeusCloud.Data
{
    public static class EntityFrameworkHelper
    {
        public static void SetMany<ChildType, ChildIDType>(
            IEnumerable<ChildType> all, List<ChildType> childs, IEnumerable<ChildIDType> newChildsIDs,
            Func<ChildType, ChildIDType> childIDSelector)
        {
            Try.Argument(newChildsIDs, nameof(newChildsIDs));

            var newChildsIDsSet = new HashSet<ChildIDType>(newChildsIDs);
            var childsIDsSet = new HashSet<ChildIDType>(childs.Select(x => childIDSelector(x)));

            childsIDsSet.IntersectWith(newChildsIDsSet);
            if (childs.Count != childsIDsSet.Count)
                childs.RemoveAll(x => !childsIDsSet.Contains(childIDSelector(x)));

            newChildsIDsSet.ExceptWith(childsIDsSet);
            if (newChildsIDsSet.Any())
                childs.AddRange(all.Where(x => newChildsIDsSet.Contains(childIDSelector(x))));
        }

    }

}