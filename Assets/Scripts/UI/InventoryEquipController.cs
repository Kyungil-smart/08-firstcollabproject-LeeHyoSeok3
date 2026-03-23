using NUnit.Framework.Interfaces;
using UnityEngine;

public class InventoryEquipController : MonoBehaviour
{
    [SerializeField] private InventoryEquipView m_view; // Inspector에서 View 스크립트 연결
    [SerializeField] private DataManager m_dataManager; // 전체 데이터 관리자

    private InventoryEquipModel m_model;

    private void Start()
    {
        // 1. Model 초기화
        m_model = new InventoryEquipModel();

        // 2. Model -> View 방향 연결 (데이터가 변하면 UI가 알아서 갱신되도록)
        m_model.OnInventoryUpdated += m_view.DrawSlots;
        m_model.OnItemSelected += m_view.ShowDetail;

        // 3. View -> Controller 방향 연결 (유저가 버튼을 누르면 Controller가 로직 수행)
        m_view.OnSlotClicked += HandleSlotClicked;
        m_view.OnEquipClicked += HandleEquipClicked;
        m_view.OnCloseClicked += ClosePopup;

        // 팝업 열기 테스트
        OpenPopup();
    }

    public void OpenPopup()
    {
        m_view.gameObject.SetActive(true);
        m_view.HideDetail(); // 초기엔 상세창 숨김

        // DataManager에서 아이템 목록을 가져와 Model에 밀어넣음
        // -> Model 이벤트 발생 -> View의 DrawSlots가 자동 실행됨
        var items = m_dataManager.GetInventoryItems();
        m_model.UpdateInventory(items);
    }

    private void HandleSlotClicked(int index)
    {
        // View에서 슬롯 클릭 이벤트가 오면, Model에게 선택된 아이템을 바꾸라고 지시함
        // -> Model 이벤트 발생 -> View의 ShowDetail이 자동 실행됨
        m_model.SelectItem(index);
    }

    private void HandleEquipClicked()
    {
        // 장착 로직 실행
        ItemData itemToEquip = m_model.GetSelectedItem();
        m_dataManager.EquipItemToParty(itemToEquip.id);

        ClosePopup();
    }

    private void ClosePopup()
    {
        m_view.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위한 이벤트 해제
        if (m_model != null)
        {
            m_model.OnInventoryUpdated -= m_view.DrawSlots;
            m_model.OnItemSelected -= m_view.ShowDetail;
        }
    }
}