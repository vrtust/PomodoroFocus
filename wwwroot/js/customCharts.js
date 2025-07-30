// 使用一个对象来存储图表实例，防止在页面重渲染时重复创建
window.chartInstances = {};

// 创建或更新饼图的函数
function createOrUpdatePieChart(canvasId, chartData) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        console.error(`Canvas with id ${canvasId} not found.`);
        return;
    }

    // 如果该 canvas 已有图表实例，先销毁它
    if (window.chartInstances[canvasId]) {
        window.chartInstances[canvasId].destroy();
    }

    // 创建新的图表实例
    window.chartInstances[canvasId] = new Chart(ctx, {
        type: 'pie', // 图表类型为饼图
        data: {
            labels: chartData.labels, // 数据的标签 (例如: "工作", "学习")
            datasets: [{
                label: '时长 (分钟)',
                data: chartData.values, // 对应的数据值
                backgroundColor: [ // 为每个切片提供一组好看的颜色
                    '#FF6384',
                    '#36A2EB',
                    '#FFCE56',
                    '#4BC0C0',
                    '#9966FF',
                    '#FF9F40'
                ],
                hoverOffset: 4
            }]
        },
        options: {
            responsive: true, // 响应式布局
            plugins: {
                legend: {
                    position: 'top', // 图例位置
                },
                tooltip: {
                    // 自定义提示框，显示分钟数
                    callbacks: {
                        label: function (context) {
                            let label = context.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.parsed !== null) {
                                label += context.parsed + ' min';
                            }
                            return label;
                        }
                    }
                }
            }
        }
    });
}

// 销毁图表的函数，在离开页面时调用
function destroyChart(canvasId) {
    if (window.chartInstances[canvasId]) {
        window.chartInstances[canvasId].destroy();
        delete window.chartInstances[canvasId];
    }
}
