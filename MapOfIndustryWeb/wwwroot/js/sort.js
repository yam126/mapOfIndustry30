/**
 * json对象数组按照某个属性排序:降序排列
 * @param {any} propertyName
 */
function compareDesc(propertyName)
{
    return (object1, object2)=>{
        var value1 = object1[propertyName];
        var value2 = object2[propertyName];
        if (value2 < value1) {
            return -1;
        } else if (value2 > value1) {
            return 1;
        } else {
            return 0;
        }
    };
}

/**
 * json对象数组按照某个属性排序:升序排列
 * @param {any} propertyName
 */
function compareAsc(propertyName)
{
    return (object1, object2)=>{
        var value1 = object1[propertyName];
        var value2 = object2[propertyName];
        if (value2 < value1) {
            return -1;
        } else if (value2 > value1) {
            return 1;
        } else {
            return 0;
        }
    };
}
