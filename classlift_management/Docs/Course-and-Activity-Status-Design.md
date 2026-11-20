# Course and Activity Status Design

本文档说明 Course 和 Activity 在不同数据层级上的状态设计，供后端、前端、报表和测试开发人员参考。

## 1. 设计原则

Course 和 Activity 都包含“系列/定义”和“具体场次”两个不同层级。不要把系列状态、场次状态和参加者报名状态混用。

```text
系列/定义
    └── 一个或多个 Session
             └── 一个或多个 participant enrollment
```

状态判断应始终结合记录类型和所属层级。特别是 Course 的 `CourseEnrollment` 表同时承载课程报名、课程 master session、participant session 等不同记录类型，不能只根据字符串状态判断业务含义。

## 2. Course 状态

### 2.1 Course 系列层级

Course 本身没有字符串 `Status` 字段，使用 `IsActive` 表示课程是否仍在系统中启用。

| 值 | 含义 |
|---|---|
| `true` | 课程仍然启用，并可显示给符合条件的 Staff、Coach 或 participant。Group Course 达到容量后仍可保持 active，只是不能继续报名。 |
| `false` | 课程停用，不再作为新的报名或排课选择。历史报名和 Session 记录仍然保留。 |

`IsActive` 不表示某个 Session 是否已经完成，也不表示某个 participant 是否已确认报名。

### 2.2 Course master Session

Group Course 的 master Session 使用以下状态：

| 状态 | 含义 | 是否可报名 |
|---|---|---|
| `Open` | 场次已创建并开放报名 | 是，受容量限制 |
| `Closed` | 场次存在，但不再接受新的报名 | 否 |
| `Completed` | 场次已结束 | 否 |
| `Canceled` | Staff 取消了场次 | 否 |
| `Deleted` | 未产生有效 participant registration 的场次被删除，或仅作为历史记录保留 | 否 |

创建 Group master Session 时通常使用 `Open`。有些编辑流程会使用 `Closed` 表示停止报名。场次完成或取消后不能重新当作开放场次使用。

### 2.3 Course participant Session

participant 的 child session copy 使用以下状态：

| 状态 | 含义 |
|---|---|
| `Registered` | participant 已登记，但根报名尚未确认，或尚未完成确认流程 |
| `Scheduled` | participant 已确认，场次已安排 |
| `Completed` | participant 已完成该 Session |
| `Canceled` | 该 participant 的 Session 被取消 |
| `OnLeave` | participant 已获准请假 |
| `RequestToLeave` | participant 正在请求请假 |
| `RequestToReschedule` | participant 正在请求改期；主要用于允许 participant 改期的 Private Course |
| `Deleted` | Session 记录被移除但需要保留历史；不再是有效安排 |

这些状态是 participant Session 的状态，不应写入 Course 系列本身。

### 2.4 Course root registration

Course root registration 是 participant 与 Course 的长期报名关系，使用：

| 状态 | 含义 |
|---|---|
| `Registered` | 已报名但尚未确认 |
| `Confirmed` | 已确认参加课程 |
| `Completed` | 课程报名关系已完成 |
| `Canceled` | 课程报名关系已取消 |
| `Deleted` | 报名记录从当前流程移除，但保留历史 |

Root registration 和 child session copy 必须分开处理。确认 root registration 通常会把其仍为 `Registered` 的 participant Session 更新为 `Scheduled`。

## 3. Activity 状态

### 3.1 Activity Series 层级

Activity Series 使用 active/inactive 生命周期，不使用 `Open`、`Closed` 或 `Completed` 表示系列状态。

| 值 | 含义 |
|---|---|
| `IsActive = true` | Series 仍然可管理，并可用于新增 Session。 |
| `IsActive = false` | Series 停用，不再用于新的运营流程；历史 Session 和报名仍保留。 |

Activity Series 的 `ContactID` 是可选的系列默认联系人。具体 Session 可以指定自己的联系人并覆盖默认值。

### 3.2 Activity Session

Activity Session 复用 Group Course master Session 的状态：

| 状态 | 含义 | 是否可报名 |
|---|---|---|
| `Open` | Session 已创建并开放报名 | 是，受该 Session 的容量限制 |
| `Closed` | Session 不再接受新的报名 | 否 |
| `Completed` | Session 已结束 | 否 |
| `Canceled` | Staff 取消了该 Session | 否 |

Activity Session 的容量、日期、时区、时长、费用、联系人和状态都是 Session 自己的属性。不能从 Activity Series 读取这些值。

### 3.3 Activity Enrollment

Activity Enrollment 是 participant 对单个 Activity Session 的报名，使用：

| 状态 | 含义 |
|---|---|
| `Registered` | participant 已报名但尚未确认 |
| `Confirmed` | participant 已确认参加该 Session |
| `Completed` | participant 已完成该 Session |
| `Canceled` | 报名或对应 Session 被取消 |
| `Deleted` | 报名不再参与当前流程，但保留历史记录 |

Activity 不需要直接复制 Course 的 `OnLeave`、`RequestToLeave` 或 `RequestToReschedule`，除非未来明确支持这些 participant workflow。

## 4. 对比总表

| 层级 | Course | Activity |
|---|---|---|
| Series/definition | `IsActive` | `IsActive` |
| 可报名的 master/session | `Open`, `Closed`, `Completed`, `Canceled` | `Open`, `Closed`, `Completed`, `Canceled` |
| Participant enrollment | `Registered`, `Confirmed`, `Completed`, `Canceled`, `Deleted` | `Registered`, `Confirmed`, `Completed`, `Canceled`, `Deleted` |
| Participant-specific session workflow | `Scheduled`, `OnLeave`, `RequestToLeave`, `RequestToReschedule`, `Deleted` 等 | 当前不额外支持，除非新增明确业务需求 |

## 5. 常见状态流程

### Course master Session

```text
Open ──> Closed
  │         │
  ├────────> Completed
  └────────> Canceled
```

### Activity Session

```text
Open ──> Closed
  │         │
  ├────────> Completed
  └────────> Canceled
```

### Activity Enrollment

```text
Registered ──> Confirmed ──> Completed
     │             │
     └─────────────┴──────> Canceled

Registered ──> Deleted
```

`Deleted` 不表示取消，也不表示完成；它表示该报名不再参与 active workflow，但仍需保留历史和财务关联。

## 6. 开发规则

1. 查询可报名 Session 时，只选择 Session 状态为 `Open`，并额外检查 Series 是否 active。
2. `Completed` 和 `Canceled` Session 不得接受新报名。
3. 停用 Series 不应删除其 Sessions、Enrollments、Fees、Payments 或 balance history。
4. 取消 Session 时，相关 active enrollments 应更新为 `Canceled`；已完成或已删除的历史记录不应被重新打开。
5. 删除报名记录时，优先使用 `Deleted` 保留历史，而不是物理删除，尤其是已经关联 Fee、Payment、Refund 或 balance history 的记录。
6. Activity Session 的容量和费用必须从 Session 读取；Activity Series 不保存这些字段。
7. 页面显示文字可以使用更友好的标签，例如“Finished”，但数据库和服务层统一使用 `Completed`。
8. 不要使用 `Finished`、`Cancelled`、`Canceled` 等多个数据库值表达同一状态；当前代码标准值是 `Canceled` 和 `Completed`。

## 7. 与费用和历史的关系

- Enrollment 的 Fee 应指向具体报名记录。
- Session 取消时，相关已付款报名可以按照 Activity 的退款规则产生 balance refund。
- `Deleted` 不应自动删除 Fee、Payment、Refund 或 balance history。
- Series 停用也不应改变已经完成或已取消的 Session 和 Enrollment 状态。

## 8. 状态值常量化建议

后续实现建议为 Course master Session、Activity Session、Course Enrollment 和 Activity Enrollment 分别建立状态常量或枚举，避免在 Controller、Repository 和 View 中散落硬编码字符串。不同层级即使共享字符串值，也应通过明确命名的常量表达其业务语义。
