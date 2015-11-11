var commData;
(function (commData) {
    commData.genderData = [
        { label: '男', value: true },
        { label: '女', value: false }
    ];
    commData.isSecondData = [
        { label: '中古', value: true },
        { label: '全新', value: false }
    ];
    commData.q_isSecondData = [
        { label: '全部', value: null },
        { label: '中古', value: true },
        { label: '全新', value: false }
    ];
    commData.equipmentCategory = [
        { key: 1, value: '加熱爐' },
        { key: 2, value: '裂解爐' },
        { key: 3, value: '熱媒鍋爐' },
    ];
    commData.queryUseType = [
        { key: 1, value: '煙氣出口溫度年平均值' },
        { key: 2, value: '爐氣含氧體積濃度年平均值' }
    ];
})(commData || (commData = {}));
