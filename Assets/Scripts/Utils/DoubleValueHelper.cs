using UnityEngine;
using Wildlife.Cheetah.ScriptableValues;

namespace Automation.Helper
{
    /**
     * Helper class to expose the Cheetah Scriptable Double Value.
     * Add this script to a game object and assign the desired scriptable
     * double value you want to expose
     */
    public class DoubleValueHelper : MonoBehaviour
    {
        [SerializeField] DoubleValue doubleValue;

        public double SavedDoubleValue()
        {
            return doubleValue.Value;
        }

        public string DoubleValueName()
        {
            return doubleValue.name;
        }
    }
}
