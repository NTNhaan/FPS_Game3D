using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class gunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public float fireRate = 0.1f;
    public int clipSize = 30;
    public int reservedAmmoCapacity = 270;

    //Variables that change throughout code
    private bool _canShoot;
    private int _currentAmmoInClip;  // current ammo in gun
    private int _ammoInReserve;  // ammo in reserve

    //Muzzleflash
    public Image muzzleFlashImage;
    public Sprite[] flashes;

    //Aiming
    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;
    public Vector3 runingLocalPosition;
    public Vector3 runningLocalRotation;
    public float aimSmoothing = 10;
    public Image crosshair;

    [Header("Mouse Settings")]
    [SerializeField] Transform playerCamera;
    public float mouseSensitivity = 1;
    private Vector2 _currentRotation;
    public float weaponSwayAmount = 0.05f;
    //Weapon Recoil
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstraints;
    public Vector2[] recoilPattern;

    private void Start()
    {
        _currentAmmoInClip = clipSize;
        _ammoInReserve = reservedAmmoCapacity;
        _canShoot = true;
    }
    private void Update()
    {
        DetermineAimAndRun();
        EnableCrosshair();
        DetermineRotation();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0)
        {
            _canShoot = false;
            _currentAmmoInClip--;
            StartCoroutine(ShootGun());
        }
        if (Input.GetKeyDown(KeyCode.R) && _currentAmmoInClip < clipSize && _ammoInReserve > 0)
        {
            int amountNeeded = clipSize - _currentAmmoInClip;  // reload amount
            if (amountNeeded >= _ammoInReserve)
            {
                _currentAmmoInClip += _ammoInReserve;
                _ammoInReserve -= amountNeeded;
            }
            else
            {
                _currentAmmoInClip = clipSize;
                _ammoInReserve -= amountNeeded;
            }
        }
    }
    private void DetermineRotation()   // Rotation of the Camera
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));  // giá trị đầu vào của chuột theo trục X và Y
        mouseAxis *= mouseSensitivity;
        _currentRotation += mouseAxis;
        _currentRotation.y = Mathf.Clamp(_currentRotation.y, -90, 90);  // giới hạn góc quay trục Y để không bị lật ngược

        transform.localPosition += (Vector3)mouseAxis * weaponSwayAmount / 1000;
        transform.root.localRotation = Quaternion.AngleAxis(_currentRotation.x, Vector3.up);
        playerCamera.transform.localRotation = Quaternion.AngleAxis(-_currentRotation.y, Vector3.right);
    }
    private void DetermineAimAndRun()    // Aiming
    {
        Vector3 target = normalLocalPosition;
        if (Input.GetMouseButton(1))
        {
            target = aimingLocalPosition;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * aimSmoothing);
            Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);
            transform.localPosition = desiredPosition;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {

            transform.localPosition = Vector3.Lerp(transform.localPosition, runingLocalPosition, Time.deltaTime * aimSmoothing);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(runningLocalRotation), Time.deltaTime * aimSmoothing);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, normalLocalPosition, Time.deltaTime * aimSmoothing);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * aimSmoothing);
        }
    }
    private void EnableCrosshair()
    {
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftShift))
        {
            crosshair.enabled = false;
        }
        else
        {
            crosshair.enabled = true;
        }
    }
    private void DeterminRecoil()  // Recoil for Gun
    {
        transform.localPosition -= Vector3.forward * 0.1f;
        if (randomizeRecoil)
        {
            float xRecoil = Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
            float yRecoil = Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);
            Vector2 recoil = new Vector2(xRecoil, yRecoil);
            _currentRotation += recoil;
        }
        else
        {
            int currentStep = clipSize + 1 - _currentAmmoInClip;
            currentStep = Mathf.Clamp(currentStep, 0, recoilPattern.Length - 1);
            _currentRotation += recoilPattern[currentStep];
        }
    }
    private IEnumerator ShootGun()
    {
        DeterminRecoil();
        StartCoroutine(MuzzlePflash());
        RaycastForEnemy();
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }
    private IEnumerator MuzzlePflash()
    {
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        muzzleFlashImage.sprite = null;
        muzzleFlashImage.color = new Color(0, 0, 0, 0);
    }
    private void RaycastForEnemy()
    {
        RaycastHit hit;  // save infomation about collisions
        Vector3 rayOrigin = playerCamera.transform.position; // startPoint of the Raycast
        Vector3 rayDirection = playerCamera.transform.forward;  // Direction of the Raycast
        Debug.DrawRay(rayOrigin, rayDirection * 100, Color.red, 2.0f);
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Enemy")))
        {
            try
            {
                Debug.Log("Hit an Enemy!");
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 forceDirection = playerCamera.transform.forward * 500;
                    rb.constraints = RigidbodyConstraints.None;
                    rb.AddForce(forceDirection);  // Aplly thrust to Regidbody of enemy
                }
            }
            catch
            {
                Debug.Log("Hit an Enemy but no Rigidbody found!");
            }
        }
        else
        {
            Debug.Log("Missed!");
        }
    }
}


