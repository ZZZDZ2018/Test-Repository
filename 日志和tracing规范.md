# **日志规范**

## 1. 基本原则
- **结构化日志**：使用JSON格式，便于机器解析
- **可追溯性**：必须包含TraceID/SpanID（关联Tracing系统）
- **等级分明**：按重要性分级记录
- **无敏感信息**：禁止记录密码、密钥、完整卡号等敏感数据

## 2. 日志等级规范
| 等级    | 使用场景                          | 示例                          |
|---------|---------------------------------|-----------------------------|
| FATAL   | 导致服务崩溃的不可恢复错误           | 数据库连接池耗尽               |
| ERROR   | 业务逻辑错误/外部依赖失败           | 支付接口调用失败               |
| WARN    | 需要关注的异常但服务仍可用           | 缓存命中率低于阈值             |
| INFO    | 关键业务流程记录                   | 订单创建成功，订单ID: 123      |
| DEBUG   | 调试信息（默认不开启）              | SQL执行耗时：42ms            |
| TRACE   | 详细链路追踪（高频日志）            | 方法入参：{userId: 456}       |

## 3. 日志格式规范
```json
{
  "timestamp": "2023-08-20T14:32:15.123Z",
  "level": "INFO",
  "traceId": "3d7284a0-5b9e-4e1f",
  "spanId": "a1b2c3d4",
  "service": "order-service",
  "class": "com.example.OrderController",
  "method": "createOrder",
  "message": "订单创建成功",
  "context": {
    "orderId": 123,
    "userId": 456,
    "amount": 99.99
  },
  "exception": {
    "type": "NullPointerException",
    "stackTrace": "..."
  }
}
```

## 4. 日志分类
| 类型       | 记录内容                     | 存储策略            |
|------------|----------------------------|-------------------|
| 业务日志    | 核心业务流程记录             | 保留30天           |
| 审计日志    | 关键数据变更记录             | 保留1年（不可删除）  |
| 访问日志    | HTTP请求记录                | 保留7天            |
| 调试日志    | 临时问题排查日志             | 保留3天            |

---

# **Tracing规范**

## 1. 基础要求
- **全链路透传**：所有服务必须传播Trace上下文
- **统一ID格式**：TraceID使用16字节HEX字符串（如：3d7284a05b9e4e1f）
- **遵循标准**：使用W3C TraceContext规范

## 2. Span定义规范
| 字段          | 规范                          | 示例                     |
|---------------|-----------------------------|------------------------|
| Span Name     | `服务名.操作类型.资源类型`       | `payment.http.post`    |
| Span Kind     | 标明Span类型                   | SERVER/CLIENT/PRODUCER |
| Attributes    | 关键业务属性                    | `order.id=123`         |
| Events        | 重要阶段标记                    | `redis.hit`            |
| Status        | 明确成功/失败                   | OK/ERROR               |

## 3. 上下文传播
```python
# HTTP请求头示例
headers = {
    "traceparent": "00-3d7284a05b9e4e1f-a1b2c3d4-01",
    "tracestate": "vendor=alibaba,version=1"
}

# 消息队列属性示例（Kafka）
headers = [
    ("trace_id", b"3d7284a05b9e4e1f"),
    ("span_id", b"a1b2c3d4")
]
```

## 4. 采样策略
| 场景              | 采样率      | 说明                     |
|-------------------|------------|------------------------|
| 生产环境           | 10%        | 错误请求100%采样          |
| 预发布环境         | 100%       | 全量采样                 |
| 调试模式           | 100%+调试标记 | 携带`x-debug-trace: true` |

## 5. 黄金指标（必须采集）
1. **延迟**（Latency）：请求处理时间
2. **流量**（Traffic）：QPS/RPS
3. **错误率**（Error Rate）：HTTP 5xx比例
4. **饱和度**（Saturation）：资源使用率

---

# **关联实现建议**

## 日志与Tracing联动
```python
# 在日志中自动注入Trace信息
import logging
from opentelemetry import trace

logger = logging.getLogger(__name__)

def log_order_created(order_id):
    current_span = trace.get_current_span()
    logger.info(
        "Order created",
        extra={
            "trace_id": current_span.context.trace_id,
            "span_id": current_span.context.span_id,
            "order_id": order_id
        }
    )
```

## 工具推荐
| 类型       | 推荐方案                      |
|------------|-----------------------------|
| 日志收集    | ELK（Elastic+Logstash+Kibana）|
| 分布式追踪  | Jaeger/Zipkin/SkyWalking    |
| 监控报警    | Prometheus+Grafana          |

---

**实施效果**：  
1. 可快速定位跨服务问题（通过TraceID串联日志）  
2. 精确分析系统瓶颈（通过Span耗时分析）  
3. 满足审计合规要求（完整操作追溯）  

**注意事项**：  
1. 避免过度日志（控制DEBUG/TRACE级别输出频率）  
2. Tracing采样需考虑业务吞吐量  
3. 敏感字段需脱敏（如手机号、身份证号）  