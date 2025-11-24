//Copyright AWAN SOFTWORKS LTD 2025

using System.Linq;
using Mono.CSharp;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Armony.Utilities.Libraries
{
    public static class LibMathematics
    {
        public static bool VectorWithinRange(this Vector3 _first, Vector3 _second, float _range = 0.001f)
        {
            return Vector3.SqrMagnitude(_first - _second) < _range * _range;
        }

        public static Vector3 RandomVec3(float _spread = 180)
        {
            return new Vector3(
                Random.Range(-_spread, _spread),
                Random.Range(-_spread, _spread),
                Random.Range(-_spread, _spread)
            );
        }

        public static float RandomPerlinFloat(float _seed = 0, float _timeScale = 1)
        {
            return Mathf.PerlinNoise(_seed, _seed + (Time.time * _timeScale));
        }

        public static float RandomPerlinFloatBalanced(float _seed = 0, float _timeScale = 1)
        {
            return (RandomPerlinFloat(_seed, _timeScale) - 0.5f) * 2f;
        }

        public static Vector3 RandomPerlinVec3(float _seed = 0, float _timeScale = 1)
        {
            return new Vector3(
                RandomPerlinFloat(_seed, _timeScale),
                RandomPerlinFloat(_seed + 1000, _timeScale),
                RandomPerlinFloat(_seed + 1000000, _timeScale)
            );
        }

        public static Vector3 RandomPerlinVec3Balanced(float _seed = 0, float _timeScale = 1)
        {
            return new Vector3(
                RandomPerlinFloatBalanced(_seed, _timeScale),
                RandomPerlinFloatBalanced(_seed + 1000, _timeScale),
                RandomPerlinFloatBalanced(_seed + 1000000, _timeScale)
            );
        }

        public static bool RandomChance(float _chance)
        {
            return Random.Range(0f, 1f) < _chance;
        }

        public static int WeightedRandom(int _upperLimit, float _decay = 0.8f)
        {
            if (_upperLimit < 1) return 0;
            _decay = Mathf.Clamp01(_decay);

            float r = Random.value;
            float maxCdf = 1f - Mathf.Pow(_decay, _upperLimit + 1);
            float value = Mathf.Log(1f - r * maxCdf) / Mathf.Log(_decay);
            int index = Mathf.FloorToInt(value);
            return Mathf.Clamp(index, 0, _upperLimit);
        }

        public static void RotateRandom(this Transform _t, float _spread)
        {
            _spread /= 2;
            _t.Rotate(RandomVec3(_spread));
        }

        public static void RotateRandomPerlin(this Transform _t, float _seed = 0, float _timeScale = 0)
        {
            _t.Rotate(RandomPerlinVec3(_seed, _timeScale));
        }

        public static void AddExplosionForce(this Rigidbody2D _rb, float _explosionForce, Vector2 _explosionPosition)
        {
            Vector2 explosionDir = _rb.position - _explosionPosition;
            _rb.AddForce(_explosionForce * explosionDir);
        }

        public static float GetImpactForceSum(this Collision2D _collision)
        {
            //http://answers.unity.com/answers/1906926/view.html
            ContactPoint2D[] contacts = new ContactPoint2D[_collision.contactCount];
            _collision.GetContacts(contacts);
            return contacts.Sum(_contact => _contact.normalImpulse);
        }

        public static void GroundObject(this Transform _transform, int _layerMask = ~0)
        {
            if (Physics.Raycast(new Ray(_transform.position, Vector3.down), out RaycastHit hit, Mathf.Infinity, _layerMask))
                _transform.position = hit.point;
        }

        public static float AngleBetweenTwoPoints(Vector3 _a, Vector3 _b)
        {
            return Mathf.Atan2(_a.y - _b.y, _a.x - _b.x) * Mathf.Rad2Deg;
        }

        public static float RoundToNearestMultiple(this float _numberToRound, float _multipleOf)
        {
            int multiple = Mathf.RoundToInt(_numberToRound / _multipleOf);
            return multiple * _multipleOf;
        }

        public static float GetDiagonality(Vector2 _vector)
        {
            if (_vector == Vector2.zero) return 0f;
            float absX = Mathf.Abs(_vector.x);
            float absY = Mathf.Abs(_vector.y);
            return Mathf.Min(absX, absY) / Mathf.Max(absX, absY);
        }

        //mathematically accurate modulus function
        public static int Modulus(this int _i, int _count) =>
            (_i % _count + _count) % _count;
    }
}