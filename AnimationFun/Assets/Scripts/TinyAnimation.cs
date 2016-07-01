using UnityEngine;
using System.Collections;

public class TinyAnimation : MonoBehaviour {

    public struct VectorCurve
    {
        public AnimationCurve x;
        public AnimationCurve y;
        public AnimationCurve z;
        public int count;
        public float duration;
    }

    //Stores animations for each transform component
    private VectorCurve vcPosition;
    private VectorCurve vcRotation;
    private VectorCurve vcScale;
    private float animDuration;

    public enum TransformMode 
    {
        POSITION,
        ROTATION,
        SCALE
    }

    public enum InterpolationMode
    {
        LINEAR,
        SMOOTH,
        LINEAR_SMOOTH //higher tangent smooth weight, creating a linear interpolation when points are aligned,
                      //but maintaining the smooth curves when there is a change in direction
    }

    //Initialize variables before the Monobehavior Start
    private void Awake()
    {
        vcPosition.x = new AnimationCurve();
        vcPosition.y = new AnimationCurve();
        vcPosition.z = new AnimationCurve();

        vcRotation.x = new AnimationCurve();
        vcRotation.y = new AnimationCurve();
        vcRotation.z = new AnimationCurve();

        vcScale.x = new AnimationCurve();
        vcScale.y = new AnimationCurve();
        vcScale.z = new AnimationCurve();
    }

    /// <summary>
    /// Interpolates the curve.
    /// </summary>
    /// <returns>The interpolated curve.</returns>
    /// <param name="mode">Mode.</param>
    /// <param name="vCurve">V curve.</param>
    private VectorCurve InterpolateCurve(InterpolationMode mode, VectorCurve vCurve)
    {
        if (mode == InterpolationMode.LINEAR)
        {
            //instantiate temporary arrays
            Keyframe[] keyframesX = new Keyframe[vCurve.count];
            Keyframe[] keyframesY = new Keyframe[vCurve.count];
            Keyframe[] keyframesZ = new Keyframe[vCurve.count];

            for (int i = 0; i < vCurve.count; i++)
            {
                //calculations are done using temporary variables
                keyframesX[i] = new Keyframe(vCurve.x.keys[i].time, vCurve.x.keys[i].value);
                keyframesY[i] = new Keyframe(vCurve.y.keys[i].time, vCurve.y.keys[i].value);
                keyframesZ[i] = new Keyframe(vCurve.z.keys[i].time, vCurve.z.keys[i].value);

                //set in and out tangents to linear mode
                SetLinearTangents(keyframesX, i, vCurve.x, vCurve.count);
                SetLinearTangents(keyframesY, i, vCurve.y, vCurve.count);
                SetLinearTangents(keyframesZ, i, vCurve.z, vCurve.count);
            }

            //Update curves
            vCurve.x = new AnimationCurve(keyframesX);
            vCurve.y = new AnimationCurve(keyframesY);
            vCurve.z = new AnimationCurve(keyframesZ);
        }

        if (mode == InterpolationMode.SMOOTH || mode == InterpolationMode.LINEAR_SMOOTH)
        {
            //smooth mode has weight 0, which causes the curve to be more fluid.
            //linear_smooth mode has weight 1, which causes curve to be more linear,
            //but maintaining the curve threatment when the line changes direction.
            float weight = (mode == InterpolationMode.LINEAR_SMOOTH) ? 1f : 0f;

            for (int i = 0; i < vCurve.count; i++)
            {
                vCurve.x.SmoothTangents(i, weight);
                vCurve.y.SmoothTangents(i, weight);
                vCurve.z.SmoothTangents(i, weight);
            }
        }
        return vCurve;
    }

    //Sets the curve keyframes linear tangents
    private void SetLinearTangents(Keyframe[] ks, int index, AnimationCurve curve, int vectorCurveLenght)
    {
        //If the index is not the last one, calculate the out tangent
        float outTangent = (index != vectorCurveLenght - 1) ? CalculateLinearTangent(curve, index, index + 1) : 0f;
        //If the index is not the first one, calculate the in tangent
        float inTangent = (index != 0) ? CalculateLinearTangent(curve, index, index - 1) : 0f;

        ks[index].outTangent = outTangent;
        ks[index].inTangent = inTangent;
    }

    //Calculates the tangent for a Liner interpolation. Used to calculate in and out tangent values between two keyframe indexes.
    private float CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
    {
        return (float) (((double) curve[index].value - (double) curve[toIndex].value) / ((double) curve[index].time - (double) curve[toIndex].time));
    }


    private Vector3 SampleCurve(float time, VectorCurve vectorCurve)
    {
        Vector3 vector = new Vector3(vectorCurve.x.Evaluate(time), vectorCurve.y.Evaluate(time), vectorCurve.z.Evaluate(time));
        return vector;
    }

    /// <summary>
    /// Sample the specified time.
    /// </summary>
    /// <param name="time">Time.</param>
    public void Sample(float time)
    {
        //makes sure that there is an animation frame to be sampled
        if (vcPosition.duration > time)
        {
            transform.localPosition = SampleCurve(time, vcPosition);
        }
        if (vcRotation.duration > time)
        {
            transform.localEulerAngles = SampleCurve(time, vcRotation);
        }
        if (vcScale.duration > time)
        {
            transform.localScale = SampleCurve(time, vcScale);
        }
    }

    /// <summary>
    /// Removes the vector keyframe.
    /// </summary>
    /// <param name="index">Index.</param>
    private void RemoveVectorKeyframe(int index, VectorCurve vectorCurve)
    {
        vectorCurve.x.RemoveKey(index);
        vectorCurve.y.RemoveKey(index);
        vectorCurve.z.RemoveKey(index);
    }

    /// <summary>
    /// Removes the key frame.
    /// </summary>
    /// <param name="index">Index.</param>
    /// <param name="transformMode">Transform mode.</param>
    public void RemoveKeyFrame(int index, TransformMode transformMode)
    {
        if (transformMode == TransformMode.POSITION)
        {
            RemoveVectorKeyframe(index, vcPosition);
            vcPosition.count--;
            vcPosition.duration = vcPosition.x.keys[index - 1].time; //the axis doesn't matter because all keyframes use the same time.
        }   
        if (transformMode == TransformMode.ROTATION)
        {
            RemoveVectorKeyframe(index, vcRotation);
            vcRotation.count--;
            vcRotation.duration = vcRotation.x.keys[index - 1].time; //the axis doesn't matter because all keyframes use the same time.
        } 
        if (transformMode == TransformMode.SCALE)
        {
            RemoveVectorKeyframe(index, vcScale);
            vcScale.count--;
            vcScale.duration = vcScale.x.keys[index - 1].time; //the axis doesn't matter because all keyframes use the same time.
        }
        CalculateDuration();
    }

    /// <summary>
    /// Adds the vector keyframe.
    /// </summary>
    /// <param name="time">Time.</param>
    /// <param name="vector">Vector.</param>
    /// <param name="vectorCurve">Vector curve.</param>
    public void AddVectorKeyframe(float time, Vector3 vector, VectorCurve vectorCurve)
    {
        vectorCurve.x.AddKey(time, vector.x);
        vectorCurve.y.AddKey(time, vector.y);
        vectorCurve.z.AddKey(time, vector.z);
    }

    /// <summary>
    /// Adds a 2D vector keyframe.
    /// </summary>
    /// <param name="time">Time.</param>
    /// <param name="vector">Vector.</param>
    /// <param name="vectorCurve">Vector curve.</param>
    public void AddVectorKeyframe(float time, Vector2 vector, VectorCurve vectorCurve)
    {
        AddVectorKeyframe(time, new Vector3(vector.x, vector.y, 0f), vectorCurve);
    }

    /// <summary>
    /// Adds the key frame.
    /// </summary>
    /// <param name="time">Time.</param>
    /// <param name="vector">Vector.</param>
    /// <param name="transformMode">Transform mode (position, rotation, scale).</param>
    public void AddKeyFrame(float time, Vector3 vector, TransformMode transformMode)
    {
        //Makes sure time cannot be negative
        if (time < 0f)
        {
            time = 0f;
            Debug.LogError("Keyframe time cannot be less than 0.", this);
        }
        switch (transformMode)
        {
            case TransformMode.POSITION:
                AddVectorKeyframe(time, vector, vcPosition);
                //tracks the duration by storing the time of the last keyframe.
                vcPosition.duration = (vcPosition.duration < time) ? time : vcPosition.duration;
                vcPosition.count++;
                break;
            case TransformMode.ROTATION:
                AddVectorKeyframe(time, vector, vcRotation);
                //tracks the duration by storing the time of the last keyframe.
                vcRotation.duration = (vcRotation.duration < time) ? time : vcRotation.duration;
                vcRotation.count++;
                break;
            case TransformMode.SCALE:
                AddVectorKeyframe(time, vector, vcScale);
                //tracks the duration by storing the time of the last keyframe.
                vcScale.duration = (vcScale.duration < time) ? time : vcScale.duration;
                vcScale.count++;
                break;
            default:
                break;
        }
        animDuration = CalculateDuration();
    }

    //Checks all transform components and find the longest duration
    private float CalculateDuration()
    {
        float duration = -99f;
        if (vcPosition.duration > duration)
        {
            duration = vcPosition.duration;
        }
        if (vcRotation.duration > duration)
        {
            duration = vcRotation.duration;
        }
        if (vcScale.duration > duration)
        {
            duration = vcScale.duration;
        }
        return duration;
    }

    /// <summary>
    /// Sets the interpolation mode.
    /// </summary>
    /// <param name="mode">Mode.</param>
    public void SetInterpolationMode(InterpolationMode mode)
    {
        InterpolateCurve(mode, vcPosition);
        InterpolateCurve(mode, vcRotation);
        InterpolateCurve(mode, vcScale);
    }

    /// <summary>
    /// Play animation.
    /// </summary>
    /// <param name="loop">If set to <c>true</c> loop.</param>
    public void Play(bool loop = false)
    {
        StartCoroutine(PlayAnimation(loop));
    }

    //Plays animation
    private IEnumerator PlayAnimation(bool loop = false)
    {
        float timeCounter = 0f;

        //stops courotine from causing a stack overload if called with looping(true)
        if (animDuration == 0f)
        {
            yield break;
        }

        while (timeCounter < animDuration)
        {
            //Sample each frame
            Sample(timeCounter);

            timeCounter += Time.deltaTime;
            yield return null;
        }

        if (loop)
        {
            StartCoroutine(PlayAnimation(loop));
        }
    }

    /// <summary>
    /// Stops the animation.
    /// </summary>
    public void StopAnimation()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    /// <returns>The animation duration.</returns>
    public float GetAnimationDuration()
    {
        return animDuration;
    }

    /// <summary>
    /// Gets the duration of the curve.
    /// </summary>
    /// <returns>The curve duration.</returns>
    /// <param name="mode">Mode.</param>
    public float GetCurveDuration(TransformMode mode)
    {
        switch (mode)
        {
            case TransformMode.POSITION:
                return vcPosition.duration;

            case TransformMode.ROTATION:
                return vcRotation.duration;

            case TransformMode.SCALE:
                return vcScale.duration;

            default:
                Debug.LogError("Unpredictable result when getting curve duration", this);
                return -1;

        }
    }

    /// <summary>
    /// Gets the curve count.
    /// </summary>
    /// <returns>The curve count.</returns>
    /// <param name="mode">Mode.</param>
    public int GetCurveCount(TransformMode mode)
    {
        switch (mode)
        {
            case TransformMode.POSITION:
                return vcPosition.count;

            case TransformMode.ROTATION:
                return vcRotation.count;

            case TransformMode.SCALE:
                return vcScale.count;

            default:
                Debug.LogError("Unpredictable result when getting curve count", this);
                return -1;
        }
    }
}
