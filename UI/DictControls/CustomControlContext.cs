using System;
using UI.DictControls;

namespace UI
{
    public class CustomControlContext
    {
        public CustomControlContext(IRuntimeContext context);
        public IRuntimeContext RuntimeContext { get; }
    }
}
