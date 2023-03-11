using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class BodyParts
{
    public GameObject bodyPartPrefab;
    public Sprite bodyPartImgs;
    public string bodyPartName;
    public float bodyAngle;
    public bool IsActive;
}

public class SexySpin : MonoBehaviour
{

    #region Variables

    public Transform Wheel;

    public float RotateDuration;
    public int AmountRotation;

    public ArabicText sexySpinResult;
    public Transform SpinWheelParent;
    public GameObject BodyPartPrefab;
    public List<BodyParts> bodyParts;

    public float SpinWheelRadius = 50;


    private bool isFirstTime;
    private float angle, x, y;
    private int bp, currentActive = 0;
    private List<int> activeIndexs = new List<int>();

    #endregion

    void Start()
    {
        GenerateSpinWheel();
    }


    public void GenerateSpinWheel()
    {

        Wheel.rotation = Quaternion.identity;
        bp = bodyParts.Count;
        angle = x = y = 0;
        currentActive = 0;
        activeIndexs.Clear();

        if (isFirstTime)
        {
            foreach (Transform n in SpinWheelParent)
            {
                n.gameObject.SetActive(false);
            }

            for (var a = 0; a < bp; a++)
            {
                if (bodyParts[a].IsActive)
                {
                    currentActive++;
                    activeIndexs.Add(a);
                }

                if (a == bp - 1)
                {
                    bp = currentActive;

                    activeIndexs.Sort();

                    for (var j = 0; j < currentActive; j++)
                    {
                        bodyParts[activeIndexs[j]].bodyPartPrefab.SetActive(true);

                        var pointX = j / (float)bp;
                        var angleX = pointX * Mathf.PI * 2;
                        x = Mathf.Sin(angleX) * SpinWheelRadius;
                        y = Mathf.Cos(angleX) * SpinWheelRadius;

                        bodyParts[activeIndexs[j]].bodyAngle = Mathf.Rad2Deg * angleX;

                        bodyParts[activeIndexs[j]].bodyPartPrefab.transform.position = GetPos(x, y);
                        bodyParts[activeIndexs[j]].bodyPartPrefab.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Rad2Deg * angleX);
                    }
                }
            }
        }
        else
        {
            for (var i = 0; i < bp; i++)
            {
                CalculateAngles(i);

                GameObject img = Instantiate(BodyPartPrefab, GetPos(x, y), Quaternion.Euler(0, 0, -Mathf.Rad2Deg * angle));

                img.transform.GetChild(0).GetComponent<Image>().sprite = bodyParts[i].bodyPartImgs;
                img.name = bodyParts[i].bodyPartName;
                bodyParts[i].bodyPartPrefab = img;

                img.transform.SetParent(SpinWheelParent);

                if (i == bp - 1)
                {
                    isFirstTime = true;
                }

            }
        }
    }


    public void CalculateAngles(int index)
    {
        Debug.Log("Calculating Angles...");

        var point = index / (float)bp;
        angle = point * Mathf.PI * 2;
        x = Mathf.Sin(angle) * SpinWheelRadius;
        y = Mathf.Cos(angle) * SpinWheelRadius;

        bodyParts[index].bodyAngle = Mathf.Rad2Deg * angle;

    }

    public Vector3 GetPos(float x, float y)
    {
        return new Vector3(x, y) + Wheel.position;
    }

    public void StartSpining()
    {
        Wheel.rotation = Quaternion.identity;

        int index = activeIndexs.Count > 0 ? activeIndexs[Random.Range(0, activeIndexs.Count)] : Random.Range(0, bp);
        string selectedBodyPart = bodyParts[index].bodyPartName;
        float selectedAngle = bodyParts[index].bodyAngle;

        float rotateAngles = (360 * AmountRotation) - selectedAngle;
        Wheel.DOLocalRotate(new Vector3(0, 0, rotateAngles * -1), RotateDuration, RotateMode.FastBeyond360).OnComplete(() =>
        {
            sexySpinResult.Text = selectedBodyPart;
        });
    }

    public void BodyPartStatus(bool status, int ID)
    {
        bodyParts[ID].IsActive = status;

        GenerateSpinWheel();
    }
}
