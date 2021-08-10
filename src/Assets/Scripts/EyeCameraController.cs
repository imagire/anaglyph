using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCameraController : MonoBehaviour
{
    Camera cam_;

    [SerializeField]
    private Director Director_;

    // Start is called before the first frame update
    void Start()
    {
        cam_ = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProjectionMatrix();
    }


    private void UpdateProjectionMatrix()
    {
        // �ڋ�Ԃ��擾
        float ww = Director_.getWorldA4Width();
        float wh = Director_.getWorldA4Height();
        Vector3 T = -Vector3.right * ww * 0.5f;
        Vector3 B = -Vector3.forward * wh * 0.5f;
        Vector3 N = -Vector3.down;

        // �r���[��Ԃɕϊ�
        Matrix4x4 Mcam = transform.worldToLocalMatrix;
        Vector3 oWorld = Mcam.MultiplyVector(Vector3.zero);
        T = Mcam.MultiplyVector(T);
        B = Mcam.MultiplyVector(B);
        N = Mcam.MultiplyVector(N);

//T = Vector3.right * ww;
//B = Vector3.up * wh;
//N = Vector3.back;

        // �T�|�[�g�萔
        float T2 = Vector3.Dot(T, T);
        float B2 = Vector3.Dot(B, B);

        // �r���[��Ԃł̒����_
        Vector3 cam_offset = gameObject.transform.position - gameObject.transform.parent.position;
        Vector3 center = Director_.getWorldA4Center() + cam_offset;
//center = new Vector3(0, 0, 1.0f);
        center = Mcam.MultiplyPoint(center);
        Vector3 oc = -center;// �����_����J�����ւ̍����x�N�g��

        // �Ǝ��r���[�s��
        Matrix4x4 Mtbn = Matrix4x4.identity;
        Mtbn.m00 = T.x; Mtbn.m01 = T.y; Mtbn.m02 = T.z;
        Mtbn.m10 = B.x; Mtbn.m11 = B.y; Mtbn.m12 = B.z;
        Mtbn.m20 = N.x; Mtbn.m21 = N.y; Mtbn.m22 = N.z;

        // �Ǝ��ˉe�s��
        const float z_near = 0.01f;
        const float z_far = 1000.0f;
        Matrix4x4 Mp = Matrix4x4.identity;
        Mp.m00 = -Vector3.Dot(oc, N) / T2;
        Mp.m11 = -Vector3.Dot(oc, N) / B2;
        Mp.m22 = -(z_near + z_far) / (z_far - z_near);
        Mp.m23 = -2.0f * z_near * z_far / (z_far - z_near);
        Mp.m32 = -1.0f;
        Mp.m33 = 0.0f;

        // �o�C�A�X
        float bias_x = -Vector3.Dot(oc, T) / T2;
        float bias_y = -Vector3.Dot(oc, B) / B2;

        // ���E�ԋ����␳
        Vector3 Vpd = gameObject.transform.localPosition.normalized *
            (-Director_.getPupillaryDistance() * 10.0f / Director_.getA4Width());// 10.0��mm��cm�̕ϊ�
        bias_x -= Vpd.x;

        Matrix4x4 Mbias = Matrix4x4.identity;
        Mbias.m02 = -bias_x / Mp.m00;// �{���͎ˉe�s��̌�̕��s�ړ��ł��邪�A�ˉe�s��̑O�ɏ�����悤�ɂ������߁A�ς�����ꏊ�ƌW���̕␳������
        Mbias.m12 = -bias_y / Mp.m11;

        Matrix4x4 Mview = Mbias * Mtbn * Mcam;
        cam_.worldToCameraMatrix = Mview;
        cam_.projectionMatrix = Mp;


        // �e�X�g
        // Vector3 test_f11 = cam_.WorldToViewportPoint(new Vector3( 250.0f, 0.0f, 250.0f));
        // Vector3 test_f10 = cam_.WorldToViewportPoint(new Vector3( 250.0f, 0.0f,-250.0f));
        // Vector3 test_f01 = cam_.WorldToViewportPoint(new Vector3(-250.0f, 0.0f, 250.0f));
        // Vector3 test_f00 = cam_.WorldToViewportPoint(new Vector3(-250.0f, 0.0f,-250.0f));

        // Vector3 test00 = cam_.WorldToViewportPoint(paperDirector_.getWorldA4Center());
        // Vector3 test_t = cam_.WorldToViewportPoint(new Vector3(2.21f, 8.71f, 1.01f));

        // �Ƃ肠�����ˉe�s��̍����ւ�
        // Matrix4x4 mat;
        // mat = Matrix4x4.Perspective(50.0f, 210.0f / 297.0f, 0.01f, 100.0f);
        // cam_.projectionMatrix = mat;
    }
}
