using TMPro;
using UnityEngine;

public class SetResolution : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    //显示器最大分辨率
    int screenWidth;
    int screenHeight;

    private void Awake()
    {
        screenWidth = Screen.currentResolution.width;
        screenHeight = Screen.currentResolution.height;
    }

    private void Start()
    {
        var resos = Screen.resolutions;
        for (int i = 0; i < resos.Length; i+=2)
        {
            var item = new TMP_Dropdown.OptionData();
            item.text = ToResolutionString(resos[i]);
            dropdown.options.Add(item);
            dropdown.value = i;
        }

        dropdown.onValueChanged.AddListener(index =>
        {
            dropdown.captionText.text = ToResolutionString(resos[2*index]);
            SetScreenResolution(resos[2*index]);
        });
    }

    private string ToResolutionString(Resolution r)
    {
        return $"{r.width}X{r.height}";
    }

    public void SetScreenResolution(Resolution resolution)
    {
        if (resolution.height >= screenHeight || resolution.width >= screenWidth)
        {
            //设置当前分辨率
            Screen.SetResolution(screenWidth, screenHeight, true);
        }
        else Screen.SetResolution(resolution.width, resolution.height, false);
    }
}