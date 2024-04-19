//AWAN SOFTWORKS LTD 2023

using UnityEngine;

namespace Armony.Utilities.Libraries
{
    public static class LibMathematics
    {
        public static Vector3 RandomVec3(float spread = 180)
        {
            return new Vector3(
                Random.Range(-spread, spread),
                Random.Range(-spread, spread),
                Random.Range(-spread, spread)
            );
        }

        public static float RandomPerlinFloat(float seed = 0, float timeScale = 1)
        {
            return Mathf.PerlinNoise(seed, seed + (Time.time * timeScale));
        }

        public static float RandomPerlinFloatBalanced(float seed = 0, float timeScale = 1)
        {
            return (RandomPerlinFloat(seed, timeScale) - 0.5f) * 2f;
        }

        public static Vector3 RandomPerlinVec3(float seed = 0, float timeScale = 1)
        {
            return new Vector3(
                RandomPerlinFloat(seed, timeScale),
                RandomPerlinFloat(seed + 1000, timeScale),
                RandomPerlinFloat(seed + 1000000, timeScale)
            );
        }

        public static Vector3 RandomPerlinVec3Balanced(float seed = 0, float timeScale = 1)
        {
            return new Vector3(
                RandomPerlinFloatBalanced(seed, timeScale),
                RandomPerlinFloatBalanced(seed + 1000, timeScale),
                RandomPerlinFloatBalanced(seed + 1000000, timeScale)
            );
        }

        public static bool RandomChance(float chance)
        {
            return Random.Range(0f, 1f) < chance;
        }

        public static void RotateRandom(this Transform t)
        {
            t.rotation = Random.rotation;
        }

        public static void RotateRandom(this Transform t, float spread)
        {
            spread /= 2;
            t.Rotate(RandomVec3(spread));
        }

        public static void RotateRandomPerlin(this Transform t, float seed = 0, float timeScale = 0)
        {
            t.Rotate(RandomPerlinVec3(seed, timeScale));
        }

        public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition)
        {
            Vector2 explosionDir = rb.position - explosionPosition;
            rb.AddForce(explosionForce * explosionDir);
        }

        public static float GetImpactForceSum(this Collision2D collision)
        {
            //http://answers.unity.com/answers/1906926/view.html
            ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contacts);
            float totalImpulse = 0;
            foreach (ContactPoint2D contact in contacts)
            {
                totalImpulse += contact.normalImpulse;
            }
            return totalImpulse;
        }

        public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }

        public static float RoundToNearestMultiple(this float numberToRound, float multipleOf)
        {
            int multiple = Mathf.RoundToInt(numberToRound / multipleOf);
            return multiple * multipleOf;
        }
    }
}
