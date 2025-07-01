using System.Threading.Tasks;
using UnityEngine;

namespace PBBox.View
{

    public class BindViewPrefabAttribute : BindViewAttributeBase
    {
        public string AssetPath { get; set; }
        public AssetLoaderTypes LoaderTypes { get; set; } = AssetLoaderTypes.Default;
        private PrefabViewFactory m_PrefabViewFactory;

        public BindViewPrefabAttribute(string id) : base(id)
        {
        }

        public override IViewFactory GetViewFactory()
        {
            if (m_PrefabViewFactory == null)
            {
                m_PrefabViewFactory = new PrefabViewFactory(AssetPath, LoaderTypes);
            }
            return m_PrefabViewFactory;
        }
    }
}