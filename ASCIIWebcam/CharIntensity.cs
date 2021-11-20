using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWebcam {
    struct CharIntensity {

        public readonly double rightIntensity, leftIntensity, topIntensity, bottomIntensity, centerIntensity;
        public readonly char Char;

        public CharIntensity(char Char, double rightIntensity, double leftIntensity, double topIntensity, double bottomIntensity, double centerIntensity) {
            this.Char = Char;
            this.rightIntensity = rightIntensity;
            this.leftIntensity = leftIntensity;
            this.topIntensity = topIntensity;
            this.bottomIntensity = bottomIntensity;
            this.centerIntensity = centerIntensity;
        }

        public double DistanceMetric(CharIntensity compar) {
            return Math.Abs(rightIntensity - compar.rightIntensity)
                + Math.Abs(leftIntensity - compar.leftIntensity)
                + Math.Abs(topIntensity - compar.topIntensity)
                + Math.Abs(bottomIntensity - compar.bottomIntensity)
                + Math.Abs(centerIntensity - compar.centerIntensity);
        }
    }
}
