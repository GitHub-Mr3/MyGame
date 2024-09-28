using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;

public class CameraHandler : MonoBehaviour
{
    /// <summary>
    /// 目标位置 （玩家Transform）
    /// </summary>
    public Transform targetTransform;
    /// <summary>
    /// 相机的位置
    /// </summary>
    public Transform cameraTransform;
    /// <summary>
    /// 相机中 心位置
    /// </summary>
    public Transform camera_PiovtTrs;

    public Transform myTransform;

    public static CameraHandler singleton;

    public float lookSpeed = 0.01f;//水平旋转的速度
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.01f;//上下旋转的速度


    private float lookAngle;//左右旋转角度
    private float pivotAngle;//上下旋转角度
    public float minimumPiovt = -35;
    public float maximumPiovt = 35;

    public InputHandler inputHandler;

    private void Awake()
    {
        singleton = this;
        myTransform = transform;

        inputHandler = FindObjectOfType<InputHandler>();
    }
    /// <summary>
    /// 相机跟随
    /// </summary>
    /// <param name="delta"></param>
    public void FollowTarget(float delta)
    {
        Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
        myTransform.position = targetPosition;
    }

    public Transform currentLockOnTarget;
    public float CameRotationSpeed = 2;
    public void HandlerCameraRotation(float delta, float mousX, float mousY)
    {
        if (inputHandler.isLockOnFlag == false && currentLockOnTarget == null)
        {

            //根据鼠标水平移动量和旋转速度，更新视角的水平旋转角度（lookAngle）。
            lookAngle += (mousX * lookSpeed) / delta;

            //根据鼠标垂直移动量和旋转速度，更新视角的垂直旋转角度（pivotAngle）。这里使用减法是因为一般情况下，当鼠标向下移动时，视角会往上翻转。
            pivotAngle -= (mousY * pivotSpeed) / delta;

            //将更新后的垂直旋转角度（pivotAngle）限制在最小值和最大值之间。
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPiovt, maximumPiovt);

            Vector3 rotation = Vector3.zero;

            //设置rotation变量的y轴分量为水平旋转角度（lookAngle）。
            rotation.y = lookAngle;

            //根据rotation创建一个目标旋转（targetRotation）的四元数。
            Quaternion targetRotation = Quaternion.Euler(rotation);


            //将摄像机的旋转设置为目标旋转，实现摄像机的水平旋转。
            //myTransform.rotation = targetRotation;
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRotation, delta / followSpeed);

            rotation = Vector3.zero;
            //设置rotation变量的x轴分量为垂直旋转角度（pivotAngle）。
            rotation.x = pivotAngle;
            //DebugMgr.Instance.Log(rotation);
            //根据更新后的rotation再次创建目标旋转。
            targetRotation = Quaternion.Euler(rotation);
            //将相机枢轴的本地旋转设置为目标旋转，实现相机枢轴的垂直旋转。
            //camera_PiovtTrs.localRotation = targetRotation;
            camera_PiovtTrs.localRotation = Quaternion.Lerp(camera_PiovtTrs.localRotation, targetRotation, delta / followSpeed);
        }
        else
        {
            DebugMgr.Instance.Log("凝视视野");
            Vector3 dir = currentLockOnTarget.position - transform.position;
            dir.Normalize();
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            //myTransform.rotation = targetRotation;
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRotation, delta * CameRotationSpeed);

            dir = currentLockOnTarget.position - camera_PiovtTrs.position;
            dir.Normalize();

            targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = targetRotation.eulerAngles;
            eulerAngle.y = 0;
            //camera_PiovtTrs.localEulerAngles = eulerAngle;
            camera_PiovtTrs.localEulerAngles = Vector3.Lerp(camera_PiovtTrs.localEulerAngles, eulerAngle, delta * CameRotationSpeed);
        }


    }

    public float maximumLockOnDistance = 30;
    List<CharacterManager> avilableTargets = new List<CharacterManager>();

    /// <summary>
    /// 设置凝视的Obj
    /// </summary>
    public void HandleLockOn()
    {
        float shortesDistance = Mathf.Infinity;
        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();
            if (character != null)
            {
                Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                float distanceForTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                if (character.transform.root != targetTransform.transform.root &&
                    //&& viewableAngle > -50 && viewableAngle < 50 &&
                    distanceForTarget <= maximumLockOnDistance)
                {
                    avilableTargets.Add(character);
                }
            }
        }
        for (int i = 0; i < avilableTargets.Count; i++)
        {
            float distanceForTarget = Vector3.Distance(targetTransform.position, avilableTargets[i].transform.position);
            if (distanceForTarget < shortesDistance)
            {
                shortesDistance = distanceForTarget;
                nearestLockOnTarget = avilableTargets[i].lockOnTransform;
            }
            //currentLockOnTarget.InverseTransformPoint
        }
    }

    public void ClearLockOnTargets()
    {
        avilableTargets.Clear();
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
    }
    public Transform nearestLockOnTarget;
}
