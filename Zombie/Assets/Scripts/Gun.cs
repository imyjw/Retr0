using System.Collections;
using UnityEngine;

// 총을 구현
public class Gun : MonoBehaviour {
    // 총의 상태를 표현하는 데 사용할 타입을 선언
    public enum State {
        Ready, // 발사 준비됨
        Empty, // 탄알집이 빔
        Reloading // 재장전 중
    }

    public State state { get; private set; } // 현재 총의 상태

    public Transform fireTransform; // 탄알이 발사될 위치

    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과

    private LineRenderer bulletLineRenderer; // 탄알 궤적을 그리기 위한 렌더러

    private AudioSource gunAudioPlayer; // 총 소리 재생기

    public GunData gunData; // 총의 현재 데이터

    private float fireDistance = 50f; // 사정거리

    public int ammoRemain = 100; // 남은 전체 탄알
    public int magAmmo; // 현재 탄알집에 남아 있는 탄알

    private float lastFireTime; // 총을 마지막으로 발사한 시점

    private void Awake() {
        // 사용할 컴포넌트의 참조 가져오기
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        bulletLineRenderer.positionCount = 2; //사용할점을두개로변경
        bulletLineRenderer.enabled = false; //라인 렌더러를 비활성화. 총을 쏠때 잠시 활성화되도록.
    }

    private void OnEnable() {
        // Gun컴포넌트가 활성화될때마다 총 상태 초기화
        ammoRemain = gunData.startAmmoRemain;
        magAmmo = gunData.magCapacity;

        state = State.Ready;
        lastFireTime = 0;
    }

    // 발사 시도
    public void Fire() {
        if(state == State.Ready && Time.time>=lastFireTime+gunData.timeBetFire)
        {
            lastFireTime= Time.time;
            Shot();
        }

    }

    // 실제 발사 처리
    private void Shot() {
        RaycastHit hit; //레이캐스트에 의한 충돌 정보 저장 컨테이너

        Vector3 hitPosition = Vector3.zero; //탄알 충돌 지점 저장 변수

        if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance)) //레이캐스트(레이의 시작점, 레이의 방향, 충돌정보컨테이너,레이충돌을검사할최대거리)
        {//레이가 어떤 물체와 충돌한 경우
            IDamageable target = hit.collider.GetComponent<IDamageable>(); //hit.colider는 충돌한 상대방 오브젝트의 콜라이더 컴포넌트. 상대방 오브젝트로부터 IDamageable타입의 컴포넌트를 가져오는데 성공햇다는 것은 해당 컴포넌트가 OnDamage()메서드를 구현한 '공격받을수잇는'오브젝트란얘기.

            if(target != null )
            {
                target.OnDamage(gunData.damage, hit.point, hit.normal); //상대방의 OnDamage함수를 실행시켜 상대방에게 대미지 주기(대미지,탄알이맞은위치,탄알이맞은표면의방향)
            }

            hitPosition = hit.point;
        }
        else
        {//레이가 어떤 물체와 충돌 안 한 경우
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;//탄알 최대사정거리 위치를 충돌 위치로
        }

        StartCoroutine(ShotEffect(hitPosition));

        magAmmo--;
        if( magAmmo <= 0)
        {
            state = State.Empty;
        }
      
    }

    // 발사 이펙트와 소리를 재생하고 탄알 궤적을 그림
    private IEnumerator ShotEffect(Vector3 hitPosition) {
        muzzleFlashEffect.Play(); //총구 화염 효과 재생

        shellEjectEffect.Play(); //탄피 배출 효과 재생

        gunAudioPlayer.PlayOneShot(gunData.shotClip); //총격 소리 재생. Play()메서드는 이미 재생중인 오디오가 있으면 정지학 ㅗ처음부터 다시 재생하기 때문에 소리가 중첩되지 않음. 정지하지 않고 중첩하여 재생하려몀ㄴ PlayOneShot()

        bulletLineRenderer.SetPosition(0,fireTransform.position); //선의 시작점은 총구의 위치

        bulletLineRenderer.SetPosition(1,hitPosition); //선의 끝점은 매개변수로 받은 충돌 위치

        bulletLineRenderer.enabled = true;        // 라인 렌더러를 활성화하여 탄알 궤적을 그림


        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);

        // 라인 렌더러를 비활성화하여 탄알 궤적을 지움
        bulletLineRenderer.enabled = false;
    }

    // 재장전 시도
    public bool Reload() {
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= gunData.magCapacity)
        {//이미 재장전중이거나 남은 탄알이 없거나 탄창에 탄알이 가득한경우 재장전불가
            return false;
        }

        StartCoroutine(ReloadRoutine());
        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine() {
        // 현재 상태를 재장전 중 상태로 전환
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);
      
        // 재장전 소요 시간 만큼 처리 쉬기
        yield return new WaitForSeconds(gunData.reloadTime);

        int ammoToFill = gunData.magCapacity - magAmmo;
        if (ammoRemain < ammoToFill)
        {//남은탄알이채워야할탄알보다적다면
            ammoToFill = ammoRemain;
        }

        magAmmo += ammoToFill;
        ammoRemain-= ammoToFill;

        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = State.Ready;
    }
}