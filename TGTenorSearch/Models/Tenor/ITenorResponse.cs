using System;
using System.Collections.Generic;
using System.Text;

namespace TGTenorSearch.Models.Tenor
{
    public interface ITenorResponse
    {
        IEnumerable<TenorResultBase>? Results { get; }
        string? Next { get; }
    }
}
