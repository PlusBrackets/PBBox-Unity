using UnityEngine;
using PBBox.View;

namespace PBBox.UI
{

    public class FallbackUIController : ControllerBase
    {
        public FallbackUIController(string id, IViewFactory viewLoader) : base(id, viewLoader)
        {
        }
    }
}