using System.Collections.Generic;
using UnityEngine;

public class NameList : MonoBehaviour
{
    // 남자 이름 리스트
    public static List<string> MaleNames = new List<string>
    {
        "김준서", "이도윤", "박예준", "최시우", "정하준", "유서준", "김지호", "오우진", "강민준", "조서진",
        "박현우", "김동현", "이지후", "백유준", "이은우", "박건우", "김주원", "조성민", "한지안", "장하민",
        "윤우빈", "최유찬", "이재윤", "박준우", "정정우", "이도현", "박민재", "조시현", "김민수", "박현준",
        "김승우", "장성준", "최재민", "이수현", "박지훈", "조시온", "김민혁", "박태준", "이이준", "정윤서",
        "한지성", "김승현", "장준혁", "박성훈", "최태현", "김건희", "박승민", "조준영", "이동준", "박서연",
        "정정민", "김주호", "이하율", "박지환", "최한결", "김우혁", "장도훈", "이태민", "조승호", "박준성",
        "최찬우", "김서율", "박하람", "이윤우", "정서원", "김현석", "박세준", "조수민", "이정후", "박민성",
        "김지율", "최우주", "이동하", "박찬희", "조현수", "김현민", "박우석", "이예성", "장민호", "박하빈",
        "최태영", "김승윤", "박준호", "정영준", "조성우", "김우진", "이윤호", "박시환", "최태훈", "김재원",
        "이하늘", "정태양", "김유빈", "박정호", "조동민", "이상우", "한정민", "김준하", "박형준", "최성호"
    };

    // 여자 이름 리스트
    public static List<string> FemaleNames = new List<string>
    {
        "김서연", "이서윤", "박지민", "최지우", "정하윤", "유예은", "김수아", "오수빈", "강예진", "조서현",
        "박서영", "김윤서", "이하은", "백소율", "이민서", "박채원", "김채은", "조다은", "한수현", "장하은",
        "윤다연", "최은서", "이지유", "박예린", "정예원", "이하린", "박지안", "조수아", "김서하", "박소현",
        "김하린", "장유진", "최윤아", "이하영", "박하은", "조시은", "김예나", "박다현", "이서하", "정소윤",
        "한채윤", "김은채", "장하율", "박지유", "최유나", "김유진", "이예서", "박서율", "조하윤", "김서희",
        "정민주", "박하윤", "김예빈", "장서아", "최민주", "이윤지", "박수민", "김은지", "조예진", "한수연",
        "김서아", "최지윤", "이유나", "박채은", "정하윤", "김다인", "이서진", "박하연", "조지은", "한나현",
        "김유나", "최다윤", "이하연", "박소윤", "정채연", "김예지", "이서율", "박다빈", "조유진", "한가연",
        "김수빈", "최하윤", "이유진", "박지현", "조민지", "김채윤", "장서윤", "박서인", "이예지", "정유리",
        "김나연", "박지수", "최수정", "이하나", "정지우", "박채린", "이윤아", "조지유", "김서진", "박하경"
    };

    // 의사 직업별 남자/여자 이름 리스트
    public static List<string> MaleDoctorNames = MaleNames;
    public static List<string> FemaleDoctorNames = FemaleNames;

    // 간호사 직업별 남자/여자 이름 리스트
    public static List<string> MaleNurseNames = MaleNames;
    public static List<string> FemaleNurseNames = FemaleNames;

    // 외래환자 직업별 남자/여자 이름 리스트
    public static List<string> MaleOutpatientNames = MaleNames;
    public static List<string> FemaleOutpatientNames = FemaleNames;

    // 입원환자 직업별 남자/여자 이름 리스트
    public static List<string> MaleInpatientNames = MaleNames;
    public static List<string> FemaleInpatientNames = FemaleNames;

    // 응급환자 직업별 남자/여자 이름 리스트
    public static List<string> MaleEmergencypatientNames = MaleNames;
    public static List<string> FemaleEmergencypatientNames = FemaleNames;

    // 인덱스
    private static int maleDoctorIndex = 0;
    private static int femaleDoctorIndex = 0;
    private static int maleNurseIndex = 0;
    private static int femaleNurseIndex = 0;
    private static int maleOutpatientIndex = 0;
    private static int femaleOutpatientIndex = 0;
    private static int maleInpatientIndex = 0;
    private static int femaleInpatientIndex = 0;
    private static int maleEmergencypatientIndex = 0;
    private static int femaleEmergencypatientIndex = 0;

    // 성별에 따른 이름 선택
    public static string GetUniqueName(Role role, bool isMale)
    {
        if (isMale)
        {
            switch (role)
            {
                case Role.Doctor:
                    maleDoctorIndex = maleDoctorIndex % MaleDoctorNames.Count;
                    return MaleDoctorNames[maleDoctorIndex++];
                case Role.Nurse:
                    maleNurseIndex = maleNurseIndex % MaleNurseNames.Count;
                    return MaleNurseNames[maleNurseIndex++];
                case Role.Outpatient:
                    maleOutpatientIndex = maleOutpatientIndex % MaleOutpatientNames.Count;
                    return MaleOutpatientNames[maleOutpatientIndex++];
                case Role.Inpatient:
                    maleInpatientIndex = maleInpatientIndex % MaleInpatientNames.Count;
                    return MaleInpatientNames[maleInpatientIndex++];
                case Role.EmergencyPatient:
                    maleEmergencypatientIndex = maleEmergencypatientIndex % MaleEmergencypatientNames.Count;
                    return MaleEmergencypatientNames[maleEmergencypatientIndex++];
                default:
                    return "NoName";
            }
        }
        else
        {
            switch (role)
            {
                case Role.Doctor:
                    femaleDoctorIndex = femaleDoctorIndex % FemaleDoctorNames.Count;
                    return FemaleDoctorNames[femaleDoctorIndex++];
                case Role.Nurse:
                    femaleNurseIndex = femaleNurseIndex % FemaleNurseNames.Count;
                    return FemaleNurseNames[femaleNurseIndex++];
                case Role.Outpatient:
                    femaleOutpatientIndex = femaleOutpatientIndex % FemaleOutpatientNames.Count;
                    return FemaleOutpatientNames[femaleOutpatientIndex++];
                case Role.Inpatient:
                    femaleInpatientIndex = femaleInpatientIndex % FemaleInpatientNames.Count;
                    return FemaleInpatientNames[femaleInpatientIndex++];
                case Role.EmergencyPatient:
                    femaleEmergencypatientIndex = femaleEmergencypatientIndex % FemaleEmergencypatientNames.Count;
                    return FemaleEmergencypatientNames[femaleEmergencypatientIndex++];
                default:
                    return "NoName";
            }
        }
    }
}