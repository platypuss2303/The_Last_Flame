using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThanhMau : MonoBehaviour
{
    public Image _thanhMau;
    public void capNhatThanhMau(float luongMauHienTai, float luongMauToiDa)
    {
        _thanhMau.fillAmount = luongMauHienTai / luongMauToiDa;
    }
}