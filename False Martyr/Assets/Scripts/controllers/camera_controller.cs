using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_controller : MonoBehaviour
{
    [SerializeField] private float camera_speed;
    [SerializeField] private room_controller room_controller;
    public Vector2Int room_index;
    private bool is_camera_shaking = false;

    void Update()
    {
        if (!is_camera_shaking)
        {
            Vector3 target_pos = room_controller.GetPositionFromGridIndex(room_index);
            target_pos = new Vector3(target_pos.x, target_pos.y, -10f);
            transform.position = Vector3.MoveTowards(transform.position, target_pos, camera_speed * Time.deltaTime);
        }
    }

    public IEnumerator camera_shake(float duration, float magnitude) //camera shake
    {
        is_camera_shaking = true; //start camera shake
        Vector3 originalpos = transform.position; //set original position
        float time_elapsed = 0f;
        while (time_elapsed < duration) //repeat until duration reached
        {
            float x = Random.Range(-1f, 1f) * magnitude; //calculate x and y shake for the frame
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = new Vector3(originalpos.x + x, originalpos.y + y, transform.position.z); //update position with shake
            time_elapsed += Time.deltaTime;
            yield return null; //wait for next frame
        }
        transform.position = originalpos; //return to original position
        is_camera_shaking = false; //end camera shake
    }

    public void cancel_camera_shake()
    {
        StopAllCoroutines();
        transform.position = room_controller.GetPositionFromGridIndex(room_index);
        is_camera_shaking = false;
    }
}
