using UnityEngine;

public static class NewInput
{
    public static bool mouseLocked
    {
        get
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    public static Vector2 mouseDelta
    {
        get
        {
            return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }
    }

    public static float mouseScroll
    {
        get
        {
            return Input.GetAxisRaw("Mouse ScrollWheel");
        }
    }

    public static Vector3 movement
    {
        get
        {
            return new Vector3(Input.GetAxisRaw("MoveX"), 0, Input.GetAxisRaw("MoveY"));
        }
    }

    public static Vector2 movementAbs
    {
        get
        {
            return new Vector2(Mathf.Abs(Input.GetAxisRaw("MoveX")), Mathf.Abs(Input.GetAxisRaw("MoveY")));
        }
    }

    public static bool pause
    {
        get
        {
            return Input.GetButtonDown("Pause");
        }
    }

    public static bool Shooting(WeaponManager weaponManager)
    {
        if (((Weapon)weaponManager.playerWeapons[weaponManager.CurrentWeaponIndex]).currentAmmoInClip > 0) 
            return Input.GetButton("Shoot"); // to prevent empty animation and sound playing every frame

        else return Input.GetButtonDown("Shoot");
    }

    public static bool reload
    {
        get
        {
            return Input.GetButtonDown("Reload");
        }
    }

    public static bool jump
    {
        get
        {
            return Input.GetButton("Jump");
        }
    }
}
