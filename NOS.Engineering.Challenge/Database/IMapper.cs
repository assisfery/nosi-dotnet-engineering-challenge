using NOS.Engineering.Challenge.Models;
using System.Collections.Generic;

namespace NOS.Engineering.Challenge.Database;

public interface IMapper<TOut, in TIn>
{
    TOut Map(Guid id, TIn item);
    TOut Patch(TOut oldItem, TIn newItem);

    //IEnumerable<TOut> FromList(List<Content> items);

    //TOut FromObject(Content item);
}