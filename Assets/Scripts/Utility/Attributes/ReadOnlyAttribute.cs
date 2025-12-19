using System;
using UnityEngine;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        // 인스펙터에서 읽기전용으로 표시하기 위한 마커
    }
}
