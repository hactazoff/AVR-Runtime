using UnityEditor;
using UnityEngine;

namespace AVR.SDK.Base
{
    public class Descriptor : MonoBehaviour
    {
        public virtual DescriptorType AssetType { get; } = DescriptorType.NONE;
        
        public SupportedPlatforms BuildTarget = SupportedPlatforms.NoTarget;
    }
}