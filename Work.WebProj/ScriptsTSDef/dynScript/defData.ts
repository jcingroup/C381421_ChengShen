module commData {
    export var genderData: labelValue<boolean>[] = [
        { label: '男', value: true },
        { label: '女', value: false }
    ];
    export var isSecondData: labelValue<boolean>[] = [
        { label: '中古', value: true },
        { label: '全新', value: false }
    ];
    export var q_isSecondData: labelValue<boolean>[] = [
        { label: '全部', value: null },
        { label: '中古', value: true },
        { label: '全新', value: false }
    ];

    //export var fuelCategory: server.FuelUnitDefine[] = [
    //    { code: 'gas', value: '氣體', unit_all_name: 'kcal/m³', unit_short_name: 'm³' },
    //    { code: 'liquid', value: '液體', unit_all_name: 'kcal/L', unit_short_name: 'kL' },
    //    { code: 'solid', value: '固體', unit_all_name: 'kcal/kg', unit_short_name: 'kg'}
    //];

    export var equipmentCategory: IKeyValue[] = [
        { key: 1, value: '加熱爐' },
        { key: 2, value: '裂解爐' },
        { key: 3, value: '熱媒鍋爐' },
    ];

    export var queryUseType: IKeyValue[] = [
        { key: 1, value: '煙氣出口溫度年平均值' },
        { key: 2, value: '爐氣含氧體積濃度年平均值' }
    ];

}