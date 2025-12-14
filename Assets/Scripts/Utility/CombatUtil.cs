namespace Utility
{
    public static class CombatUtil
    {
        public static float ApplyDef(float dmg, float def)
        {
            return dmg * (100 / (def + 100));
        }
        
        
    }
}