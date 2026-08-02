using System;
using System.Diagnostics;
using Extensions;
using UnityEngine;

namespace Utilities
{
    public static class CommonFunctions
    {
        public static string GetTimeStr( int timeSec)
        {
            int hours = timeSec / 60 / 60;
            int minutes = timeSec / 60 - hours * 60;
            int seconds = timeSec - minutes * 60 - hours * 60 * 60;

            string minStr = minutes.ToString();
            string secStr = seconds.ToString();

            if (minutes < 10)
                minStr = "0" + minutes.ToString();

            if (seconds < 10)
                secStr = "0" + seconds.ToString();

            if (hours == 0)
                return minStr + ":" + secStr;
            else
                return hours.ToString() + ":" + minStr + ":" + secStr;
        }

        public static string GetCountStringDividedByDot(int count, string separator = " ")
        {
            var countStr = count.ToString();

            if (countStr.Length <= 3)
                return countStr;

            var result = "";

            int dotCounter = 0;
            for (int i = countStr.Length - 1; i >= 0; i--)
            {
                var numberStr = countStr[i].ToString();
                dotCounter++;

                if (dotCounter >= 3 && i > 0)
                {
                    numberStr = separator + numberStr;
                    dotCounter = 0;
                }

                result = numberStr + result;
            }

            return result;
        }
    
        public static Vector3[] BuildFlyCurve(Vector3 startPoint, Vector3 endPoint, float curveAngle, float curveProcentsFromLength, float centerPointDivider = 2.0f)
        {
            Vector3 moveDir = endPoint - startPoint;
            float moveDistance = moveDir.magnitude;
            moveDir = moveDir.normalized;

            Vector3 centerPoint = startPoint + moveDir * moveDistance / centerPointDivider;
            moveDir = Quaternion.Euler(0, 0, curveAngle) * moveDir;
            centerPoint += moveDir * moveDistance * curveProcentsFromLength;

            Vector3[] positions = { startPoint, centerPoint, centerPoint, endPoint };
            return positions;
        }
        
        public static void StopAllParticleSystems(GameObject obj, float setLifeTime = 1.0f,
            bool simulateOnLifeTime = false)
        {
            var psRoot = obj.GetComponent<ParticleSystem>();
            if (psRoot)
                StopParticleSystems(psRoot, setLifeTime, simulateOnLifeTime);

            foreach (Transform child in obj.transform)
            {
                var ps = child.gameObject.GetComponent<ParticleSystem>();
                if (ps)
                    StopParticleSystems(ps, setLifeTime, simulateOnLifeTime);

                StopAllParticleSystems(child.gameObject, setLifeTime, simulateOnLifeTime);
            }
        }
        private static void StopParticleSystems(ParticleSystem ps, float setLifeTime = 1.0f,
            bool simulateOnLifeTime = false)
        {
            if (!ps) 
                return;
            
            ps.Stop();
            if (setLifeTime > 0)
            {
                var particles = new ParticleSystem.Particle[ps.particleCount];
                var num = ps.GetParticles(particles);
                for (var i = 0; i < num; i++)
                {
                    if (particles[i].remainingLifetime > setLifeTime)
                        particles[i].remainingLifetime = setLifeTime;
                }

                ps.SetParticles(particles, num);
            }

            if (simulateOnLifeTime)
                ps.Simulate(setLifeTime * 2.0f, true, false);
        }

        public static void StartProcess(string process)
        {
            var proc = new ProcessStartInfo()
            {
                UseShellExecute = true,
                WorkingDirectory = @"C:\Windows\System32",
                FileName = @"C:\Windows\System32\cmd.exe",
                Arguments = "/c " + process,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(proc);
        }

    }
}
