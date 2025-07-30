// ʹ��һ���������洢ͼ��ʵ������ֹ��ҳ������Ⱦʱ�ظ�����
window.chartInstances = {};

// ��������±�ͼ�ĺ���
function createOrUpdatePieChart(canvasId, chartData) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        console.error(`Canvas with id ${canvasId} not found.`);
        return;
    }

    // ����� canvas ����ͼ��ʵ������������
    if (window.chartInstances[canvasId]) {
        window.chartInstances[canvasId].destroy();
    }

    // �����µ�ͼ��ʵ��
    window.chartInstances[canvasId] = new Chart(ctx, {
        type: 'pie', // ͼ������Ϊ��ͼ
        data: {
            labels: chartData.labels, // ���ݵı�ǩ (����: "����", "ѧϰ")
            datasets: [{
                label: 'ʱ�� (����)',
                data: chartData.values, // ��Ӧ������ֵ
                backgroundColor: [ // Ϊÿ����Ƭ�ṩһ��ÿ�����ɫ
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
            responsive: true, // ��Ӧʽ����
            plugins: {
                legend: {
                    position: 'top', // ͼ��λ��
                },
                tooltip: {
                    // �Զ�����ʾ����ʾ������
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

// ����ͼ��ĺ��������뿪ҳ��ʱ����
function destroyChart(canvasId) {
    if (window.chartInstances[canvasId]) {
        window.chartInstances[canvasId].destroy();
        delete window.chartInstances[canvasId];
    }
}
