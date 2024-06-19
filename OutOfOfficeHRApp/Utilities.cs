using Microsoft.AspNetCore.Mvc.Rendering;

namespace OutOfOfficeHRApp
{
    public enum Status
    {
        New,
        Approved,
        Rejected,
        Submitted,
        Cancelled
    }

    public class Utilities
    {

        public static SelectList CreateSelectList(IEnumerable<object> items, string dataValue, string dataText)
        {
            return new SelectList(items, dataValue, dataText);
        }

    }
}
