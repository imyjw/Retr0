using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f; // 앞뒤 움직임의 속도
    public float rotateSpeed = 180f; // 좌우 회전 속도


    private PlayerInput playerInput; // 플레이어 입력을 알려주는 컴포넌트
    private Rigidbody playerRigidbody; // 플레이어 캐릭터의 리지드바디
    private Animator playerAnimator; // 플레이어 캐릭터의 애니메이터

    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        // 사용할 컴포넌트들의 참조를 가져오기
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    private void FixedUpdate() {
        // 물리 갱신 주기마다 움직임, 회전, 애니메이션 처리 실행
        Rotate();

        Move();

        playerAnimator.SetFloat("Move", playerInput.move);
    }

    // 입력값에 따라 캐릭터를 앞뒤로 움직임
    private void Move() {
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime; //한 프레임동안 현재 위치에서 상대적으로 더 이동할 거리와 방향 계산
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance); //MovePosition은 상대 위치가 아닌 전역 위치를 사용하기 때문에 현재위치+moveDistance
        //만약 transform.position=transform.position+moveDistance로 바꾼다면 장애물이 있어도 그 안으로 순간이동 하는 사고가 발생가능

    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    private void Rotate() {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0f); //어떤 회전상태에서 상대적으로 더 회전할때는 쿼터니언곱을 사용.
    }
}